const candidateId = document.getElementById("candidateId")?.value; // put hidden input in layout
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`/evaluationHub?candidateId=${candidateId}`)
    .build();

connection.on("EvaluationCompleted", function (examTitle) {
    console.log("Exam evaluated:", examTitle);

    // Update badge
    const badge = document.getElementById("notificationCount");
    const evaluationUrl = document.getElementById("evaluationUrl").value; 
    let count = parseInt(badge.textContent || "0") + 1;
    badge.textContent = count;
    badge.style.display = "inline";

    // Add to list
    const list = document.getElementById("notificationList");
    const item = document.createElement("div");
    item.className = "p-2 border-bottom";
    //concat a vriable 
    item.innerHTML = `<a href="${evaluationUrl}?id=${examTitle}" >Evaluation completed</a>`
   console.log(item.innerHTML)   
    list.prepend(item);

    // Optional: toast
    // toastr.success(examTitle + " evaluated");
});

connection.start().catch(err => console.error(err));

// Toggle list
document.getElementById("examNotificationBell")?.addEventListener("click", () => {
    const list = document.getElementById("notificationList");
    list.style.display = list.style.display === "none" ? "block" : "none";
});