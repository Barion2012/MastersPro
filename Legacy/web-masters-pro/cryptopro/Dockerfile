# Базовый образ
FROM debian:bookworm-slim

# Установка timezone
ENV TZ="Europe/Moscow"
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Установка зависимостей
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    openssl \
    libssl-dev \
    python3 \
    python3-pip \
    python3-venv \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Добавление пакета КриптоПро CSP 5.0 R3
COPY dist/linux-amd64_deb.tgz /tmp/src/

# Проверка наличия файла и установка КриптоПро CSP
RUN if [ ! -f /tmp/src/linux-amd64_deb.tgz ]; then \
        echo "Ошибка: файл linux-amd64_deb.tgz не найден в /tmp/src"; \
        exit 1; \
    fi && \
    cd /tmp/src && \
    tar -xf linux-amd64_deb.tgz && \
    linux-amd64_deb/install.sh && \
    # Создание симлинков для утилит
    cd /bin && \
    ln -s /opt/cprocsp/bin/amd64/certmgr && \
    ln -s /opt/cprocsp/bin/amd64/cpverify && \
    ln -s /opt/cprocsp/bin/amd64/cryptcp && \
    ln -s /opt/cprocsp/bin/amd64/csptest && \
    ln -s /opt/cprocsp/bin/amd64/csptestf && \
    ln -s /opt/cprocsp/bin/amd64/der2xer && \
    ln -s /opt/cprocsp/bin/amd64/inittst && \
    ln -s /opt/cprocsp/bin/amd64/wipefile && \
    ln -s /opt/cprocsp/sbin/amd64/cpconfig && \
    # Проверка работоспособности cryptcp
    cryptcp --help && \
    # Удаление только временных файлов
    rm -rf /tmp/src/linux-amd64_deb.tgz /tmp/src/linux-amd64_deb

# Создание виртуального окружения и установка FastAPI и uvicorn
RUN python3 -m venv /venv && \
    /venv/bin/pip install --upgrade pip && \
    /venv/bin/pip install fastapi uvicorn

# Создание директории для микросервиса
RUN mkdir -p /app

# Добавление микросервиса
COPY app.py /app/

# Импортируем в хранилище КриптоПро

COPY cert.pfx /app/cert.pfx

RUN certmgr -install -file "/app/cert.pfx" -pfx -pin "123456" -silent

WORKDIR /app

# Проверка наличия app.py
RUN if [ ! -f /app/app.py ]; then \
        echo "Ошибка: файл app.py не найден в /app"; \
        exit 1; \
    fi

# Установка пути для использования виртуального окружения
ENV PATH="/venv/bin:$PATH"

# Запуск микросервиса
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "80"]