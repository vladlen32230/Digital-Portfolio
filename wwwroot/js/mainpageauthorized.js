fetch("api/getuserinfo", {
    method: "GET"
}).then(async (response) => {
    if (response.ok) {
        const userinfo = await response.json();
        document.getElementById("searchB").onclick = (() => window.location = "search");
        const userA = document.getElementById("userA");
        userA.innerHTML = userinfo.name;
        userA.setAttribute("href", "u/" + userinfo.id);
    }
});