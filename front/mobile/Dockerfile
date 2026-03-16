FROM node:20-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 8081

RUN npm install -g @expo/ngrok

CMD ["npx", "expo", "start"]
