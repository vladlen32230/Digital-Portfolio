document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
document.getElementById("enterB").onclick = (() => {
    const data = new FormData;
    data.append("Email", document.getElementById("emailI").value);
    data.append("Password", document.getElementById("passwordI").value);
    fetch("api/enteracc", {
        method: "POST",
        body: data
    }).then(async (response) => {
        if (!response.ok) {
            const statusL = document.getElementById("statusL");
            statusL.innerHTML = await response.text();
            setTimeout(() => statusL.innerHTML = "", 3000);
        }

        else
            window.location.href = "/";
    });
});