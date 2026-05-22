FROM nginx:stable-alpine-slim

# предотвращает появление файлов, принадлежащих root на хосте
ARG PUID=1001
ARG PGID=1001

# Создаём группу и пользователя с заданными UID/GID
RUN addgroup -g ${PGID} -S authgroup \
	&& adduser -S -u ${PUID} -G authgroup authuser

WORKDIR /app

# Копируем конфигурацию nginx
COPY gateway.conf /etc/nginx/conf.d/default.conf

# Выставляем владельца и базовые права — чтобы сервис писал только в ожидаемые места
RUN mkdir -p /var/www && \
	chown -R authuser:authgroup /var/cache/nginx /var/run /var/log/nginx /etc/nginx /var/www && \
	chmod -R 0755 /etc/nginx /var/www

EXPOSE 9090
# Запускаем nginx
CMD ["nginx", "-g", "daemon off;"]