const threadId = document.getElementById("threadId").value;
const currentUserId = document.getElementById("currentUserId").value;
const messageList = document.getElementById("messageList");
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`/chathub?threadId=${threadId}`)
    .withAutomaticReconnect()
    .build();

connection.onreconnecting(error => {
    document.getElementById("messageInput").disabled = true;
    const li = document.createElement("div");
    li.textContent = `Connection lost due to error "${error}". Reconnecting.`;
    messageList.appendChild(li);
});

connection.onreconnected(connectionId => {
    document.getElementById("messageInput").disabled = false;
    const li = document.createElement("div");
    li.textContent = `Connection reestablished. Connected with connectionId "${connectionId}".`;
    messageList.appendChild(li);
});

connection.on("ReceiveMessage", function (userName, messageContent, timestamp, userId) {
    const timeFormatted = new Date(timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const messageDiv = document.createElement("div");

    if (userId !== currentUserId) {
        messageDiv.innerHTML = `
                    <div class="flex justify-start">
                        <div class="mb-2">
                            <span class="mb-1 block text-xs text-gray-600">${userName}</span>
                            <div class="max-w-xs rounded-lg bg-gray-200 p-2">
                                <p class="text-sm text-gray-800">${messageContent}</p>
                                <span class="block text-xs text-gray-500">${timeFormatted}</span>
                            </div>
                        </div>
                    </div>`;
    } else {
        messageDiv.innerHTML = `
                    <div class="flex justify-end">
                        <div class="mb-2">
                            <span class="mb-1 block text-right text-xs text-gray-600">You</span>
                            <div class="max-w-xs rounded-lg bg-blue-500 p-2 text-white">
                                <p class="text-sm">${messageContent}</p>
                                <span class="block text-xs text-gray-200">${timeFormatted}</span>
                            </div>
                        </div>
                    </div>`;
    }

    document.getElementById("messageList").appendChild(messageDiv);
    messageList.scrollTop = messageList.scrollHeight;
});
document.getElementById("loadingIndicator").classList.remove("hidden");
connection.start().then(function () {
    connection.invoke("GetThreadMessages", threadId).then(function (messages) {
        document.getElementById("loadingIndicator").classList.add("hidden");
        messages.forEach(function (message) {
            const { content, timestamp, userId } = message;
            const timeFormatted = new Date(timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
            const messageDiv = document.createElement("div");

            if (userId !== currentUserId) {
                messageDiv.innerHTML = `
                            <div class="flex justify-start">
                                <div class="mb-2">
                                    <span class="mb-1 block text-xs text-gray-600">${message.user.firstName}</span>
                                    <div class="max-w-xs rounded-lg bg-gray-200 p-2">
                                        <p class="text-sm text-gray-800">${content}</p>
                                        <span class="block text-xs text-gray-500">${timeFormatted}</span>
                                    </div>
                                </div>
                            </div>`;
            } else {
                messageDiv.innerHTML = `
                            <div class="flex justify-end">
                                <div class="mb-2">
                                    <span class="mb-1 block text-right text-xs text-gray-600">You</span>
                                    <div class="max-w-xs rounded-lg bg-blue-500 p-2 text-white">
                                        <p class="text-sm">${content}</p>
                                        <span class="block text-xs text-gray-200">${timeFormatted}</span>
                                    </div>
                                </div>
                            </div>`;
            }

            document.getElementById("messageList").appendChild(messageDiv);
            messageList.scrollTop = messageList.scrollHeight;
        });
    }).catch(function (err) {
        document.getElementById("loadingIndicator").classList.add("hidden");
        console.error("Failed to load messages: ", err);
    });
}).catch(function (err) {
    document.getElementById("loadingIndicator").classList.add("hidden");
    return console.error(err.toString());
});
document.getElementById("messageInput").addEventListener("input", function () {
    const sendButton = document.getElementById("sendButton");
    sendButton.disabled = this.value.trim() === "";
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = document.getElementById("messageInput").value;
    const user1 = document.getElementById("userId1").value;
    const user2 = document.getElementById("userId2").value;
    const title = document.getElementById("title").value;

    connection.invoke("SendMessageToThread", threadId, user1, user2, message, title)
        .then(() => {
            messageInput.value = "";
        })
        .catch(function (err) {
            console.error(err.toString());
        });
    event.preventDefault();
});

