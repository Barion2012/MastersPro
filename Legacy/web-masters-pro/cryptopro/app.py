from fastapi import FastAPI, Body, HTTPException
import subprocess
import os
import json
import shutil
import uuid
import logging
import base64
import re

app = FastAPI()

PFX_PATH = "/app/cert.pfx"  # Путь к вашему .pfx файлу
PFX_PIN = "123456"  # Пароль для .pfx файла
SIGNATURE_OUTPUT_DIR = "/app/signatures"  # Постоянная директория для хранения файлов

# Настройка логирования
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

# Функция для очистки директории
def clear_signature_directory():
    if os.path.exists(SIGNATURE_OUTPUT_DIR):
        logger.debug(f"Очистка директории {SIGNATURE_OUTPUT_DIR}")
        shutil.rmtree(SIGNATURE_OUTPUT_DIR)
    os.makedirs(SIGNATURE_OUTPUT_DIR)
    os.chmod(SIGNATURE_OUTPUT_DIR, 0o777)
    logger.debug(f"Директория {SIGNATURE_OUTPUT_DIR} создана с правами 777")

# Создаем директорию при старте приложения
if not os.path.exists(SIGNATURE_OUTPUT_DIR):
    os.makedirs(SIGNATURE_OUTPUT_DIR)
    os.chmod(SIGNATURE_OUTPUT_DIR, 0o777)

@app.post("/sign")
async def sign_content(payload: dict = Body(...)):
    try:
        # Проверка наличия файла .pfx
        if not os.path.exists(PFX_PATH):
            raise HTTPException(status_code=400, detail="Файл .pfx не найден")
        
        content = payload["content"]
        
        # Если content не строка, преобразуем в компактную JSON-строку без пробелов
        if not isinstance(content, str):
            content = json.dumps(content, ensure_ascii=False, separators=(",", ":"))
        else:
            # Если content — строка, удаляем пробелы, табуляции и переносы строк
            content = re.sub(r'\s+', '', content)
        
        if not isinstance(content, str):
            content = json.dumps(content, ensure_ascii=False)

        # Очищаем директорию перед созданием новых файлов
        clear_signature_directory()

        # Генерируем уникальное имя для файлов на основе UUID
        unique_id = str(uuid.uuid4())
        content_file = os.path.join(SIGNATURE_OUTPUT_DIR, f"content_{unique_id}.json")
        signature_file = os.path.join(SIGNATURE_OUTPUT_DIR, f"content_{unique_id}.json.sgn")

        # Сохраняем содержимое в файл
        with open(content_file, "w", encoding="utf-8") as f:
            f.write(content)
        os.chmod(content_file, 0o666)
        logger.debug(f"Файл содержимого создан: {content_file}")

        # Получаем отпечаток сертификата из .pfx с помощью certmgr
        certmgr_cmd = [
            "certmgr",
            "-list",
            "-file", PFX_PATH,
            "-pin", PFX_PIN,
            "-pfx"
        ]
        logger.debug(f"Выполняется команда certmgr: {' '.join(certmgr_cmd)}")
        result = subprocess.run(
            certmgr_cmd,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            check=True
        )

        # Парсинг отпечатка пользовательского сертификата (с закрытым ключом)
        thumbprint = None
        lines = result.stdout.splitlines()
        for i, line in enumerate(lines):
            if "PrivateKey Link     : Yes" in line:
                for j in range(i - 1, -1, -1):
                    if "SHA1 Thumbprint" in lines[j]:
                        thumbprint = lines[j].split(":")[-1].strip().replace(" ", "")
                        break
                break
        if not thumbprint:
            raise HTTPException(status_code=500, detail="Не удалось найти сертификат с закрытым ключом")

        # Подписываем через cryptcp
        cryptcp_cmd = [
            "cryptcp",
            "-sign",
            "-thumbprint", thumbprint,
            "-detached",
            "-nochain",  # Отключаем проверку цепочки сертификатов
            content_file
        ]
        logger.debug(f"Выполняется команда cryptcp: {' '.join(cryptcp_cmd)}")
        result = subprocess.run(
            cryptcp_cmd,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            check=True
        )

        # Проверяем содержимое директории
        dir_contents = os.listdir(SIGNATURE_OUTPUT_DIR)
        logger.debug(f"Содержимое директории {SIGNATURE_OUTPUT_DIR}: {dir_contents}")

        # Проверяем, создался ли файл подписи
        if not os.path.exists(signature_file):
            raise HTTPException(
                status_code=500,
                detail=f"Файл подписи {signature_file} не создан. Вывод cryptcp: stdout={result.stdout}, stderr={result.stderr}, код возврата={result.returncode}"
            )

        # Устанавливаем права на файл подписи
        os.chmod(signature_file, 0o666)
        logger.debug(f"Файл подписи создан: {signature_file}")

        try:
            # Пробуем прочитать как текст (base64) в UTF-8
            with open(signature_file, "r", encoding="utf-8") as f:
                signature = f.read()
            # Удаляем пробелы, табуляции, переносы строк
            signature = re.sub(r'\s+', '', signature)
        except UnicodeDecodeError:
            # Если файл бинарный (например, DER), читаем как байты и кодируем в base64
            with open(signature_file, "rb") as f:
                signature = base64.b64encode(f.read()).decode("utf-8")
            signature = re.sub(r'\s+', '', signature)

        # Проверяем, является ли подпись корректной base64-строкой
        if not re.match(r'^[A-Za-z0-9+/=]+$', signature):
            raise HTTPException(
                status_code=500,
                detail=f"Файл подписи {signature_file} содержит некорректные данные (не base64)"
            )

        # Перезаписываем файл подписи в компактном виде
        with open(signature_file, "w", encoding="utf-8") as f:
            f.write(signature)
        os.chmod(signature_file, 0o666)
        logger.debug(f"Файл подписи перезаписан в компактном виде: {signature_file}")

        logger.debug(f"Подпись файла: {signature}")
        logger.debug(f"Контент: {content}")

        # Возвращаем подпись и пути к файлам
        return {
            "signature": signature,
            "content_file": content_file,
            "signature_file": signature_file
        }

    except subprocess.CalledProcessError as e:
        logger.error(f"Ошибка cryptcp: {e.stderr}")
        if "No key pair" in e.stderr:
            raise HTTPException(status_code=500, detail="Закрытый ключ не найден в хранилище")
        if "License" in e.stderr:
            raise HTTPException(status_code=500, detail="Проблема с лицензией КриптоПро")
        if "0x20000066" in e.stderr:
            raise HTTPException(
                status_code=500,
                detail="Цепочка сертификатов не доверяется. Установите корневой сертификат в хранилище mroot."
            )
        raise HTTPException(
            status_code=500,
            detail=f"Ошибка подписания: код={e.returncode}, stdout={e.stdout}, stderr={e.stderr}"
        )
    except Exception as e:
        logger.error(f"Общая ошибка: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))