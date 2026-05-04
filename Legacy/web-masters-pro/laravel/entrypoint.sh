#!/bin/bash
set -e

if ! id -u www-data >/dev/null 2>&1; then
    adduser -D -S -s /bin/sh -G www-data www-data
fi

# Настраиваем права от root для storage и bootstrap/cache
chown -R www-data:www-data /var/www/html/storage /var/www/html/bootstrap/cache
chmod -R 775 /var/www/html/storage /var/www/html/bootstrap/cache

# Создаём директорию для логов Supervisor
mkdir -p /var/www/html/storage/logs/supervisor
touch /var/www/html/storage/logs/supervisor/supervisord.log
touch /var/www/html/storage/logs/supervisor/supervisord.pid
chown -R www-data:www-data /var/www/html/storage/logs/supervisor
chmod -R 775 /var/www/html/storage/logs/supervisor

# Оптимизируем Laravel
if [ -f /var/www/html/artisan ]; then
    su-exec www-data php /var/www/html/artisan optimize
fi

# Запускаем Supervisor от www-data
exec supervisord -n -c /etc/supervisor/conf.d/supervisord.conf