FROM node:22.13-alpine as builder

WORKDIR /var/www/html
COPY package*.json ./

RUN npm install && \
    npm cache clean --force

COPY ./entrypoint.sh /
RUN chmod +x /entrypoint.sh

FROM node:22.13-alpine

WORKDIR /var/www/html

COPY --from=builder /var/www/html/node_modules ./node_modules
COPY --from=builder /entrypoint.sh /

EXPOSE 3000
EXPOSE 24678
EXPOSE 5173

ENTRYPOINT ["/entrypoint.sh"]