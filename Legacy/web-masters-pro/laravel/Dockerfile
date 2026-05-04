FROM php:8.4-fpm-alpine

# Установка необходимых библиотек и компиляция расширений PHP
RUN apk update \
    && apk add --no-cache \
        supervisor \
        su-exec \
        libpng-dev \
        libjpeg-turbo-dev \
        freetype-dev \
        libzip-dev \
        oniguruma-dev \
        postgresql-dev \
        nodejs \
        npm \
        zlib \
        zlib-dev \
        openssl \ 
        ca-certificates \
    && apk add --no-cache --virtual .build-deps \
        build-base \
        autoconf \
    && pecl install redis \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install pdo_mysql mbstring zip exif pcntl gd pdo_pgsql \
    && docker-php-ext-enable redis \
    && apk del .build-deps \
    && apk cache clean \
    && rm -rf /var/cache/apk/*

# Копируем Composer
COPY --from=composer:2.7.6 /usr/bin/composer /usr/bin/composer

# Устанавливаем npm до версии 10
RUN npm install -g npm@10

# Устанавливаем рабочую директорию и копируем проект
WORKDIR /var/www/html
COPY . .

# Установка сертификатов Сбербанка
COPY /certs/crt /usr/local/share/ca-certificates

RUN update-ca-certificates

# Устанавливаем зависимости проекта и настраиваем права доступа
RUN composer install --no-interaction --prefer-dist --optimize-autoloader \
    && npm install \
    && npm run build 

# Копируем и настраиваем entrypoint.sh
COPY ./entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Используем sh для запуска entrypoint.sh
ENTRYPOINT ["/bin/sh", "/entrypoint.sh"]