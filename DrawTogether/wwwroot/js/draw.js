const userId = '@Model.UserId';
console.log("User ID:", userId);

const canvas = document.getElementById('drawCanvas');
const ctx = canvas.getContext('2d');
let drawing = false;
let prevX = 0, prevY = 0;
let currentRoom = null;

let currentColor = "black"; // Стандартный цвет
let currentTool = "pen"; // Стандартный инструмент (карандаш)
let brushSize = 5; // Стандартный размер кисти

// Слушаем изменения цвета, инструмента и размера кисти
document.getElementById('colorPicker').addEventListener('input', (e) => {
    currentColor = e.target.value;
});

document.getElementById('toolSelect').addEventListener('change', (e) => {
    currentTool = e.target.value;
});

document.getElementById('brushSize').addEventListener('input', (e) => {
    brushSize = e.target.value;
});

// Подключение к SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/drawHub")
    .build();

connection.start().then(() => {
    console.log("✅ Connected to SignalR");
});

connection.on("ReceiveDraw", (startX, startY, endX, endY, color, tool, size) => {
    drawLine(startX, startY, endX, endY, color, tool, size);
});


function findPartner() {
    document.getElementById("status").innerText = "🔍 Идёт поиск...";
    connection.invoke("StartMatchmaking", userId);
}

connection.on("WaitingForPartner", () => {
    document.getElementById("status").innerText = "⌛ Ожидание второго пользователя...";
});

connection.on("MatchFound", (roomId) => {
    currentRoom = roomId;
    document.getElementById("status").innerText = "✅ Партнёр найден! Комната: " + roomId;
    connection.invoke("JoinRoom", roomId);
});

canvas.addEventListener("mousedown", (e) => {
    drawing = true;
    [prevX, prevY] = [e.offsetX, e.offsetY];
});

canvas.addEventListener("mouseup", () => {
    drawing = false;
});

canvas.addEventListener("mousemove", (e) => {
    if (!drawing || !currentRoom) return;

    const [currX, currY] = [e.offsetX, e.offsetY];
    drawLine(prevX, prevY, currX, currY, currentColor, currentTool, brushSize); // используем выбранный цвет, инструмент и размер

    connection.invoke("SendDraw", currentRoom, prevX, prevY, currX, currY, currentColor, currentTool, brushSize);

    [prevX, prevY] = [currX, currY];
});


function drawLine(x1, y1, x2, y2, color, tool, size) {
    ctx.strokeStyle = color;
    ctx.lineWidth = size;

    if (tool === 'eraser') {
        ctx.globalCompositeOperation = 'destination-out'; // Ластик
    } else {
        ctx.globalCompositeOperation = 'source-over'; // Нормальное рисование
    }

    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
}