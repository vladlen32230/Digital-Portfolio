const user_id = window.location.pathname.split("/")[2];
const portfolio_id = window.location.pathname.split("/")[4];
document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
fetch("/api/u/" + user_id + "/p/" + portfolio_id + "/getinfo", {
    method: "GET"
}).then(async (response) => {
    if (response.ok) {
        const data = response.json();
        document.getElementById("photoIm").setAttribute("src", data.photo);
        document.getElementById("nameL").setAttribute(data.name);
        document.getElementById("descriptionL").setAttribute(data.description);
        document.getElementById("ratingL").innerHTML = data.rating;
        document.getElementById("authorA").innerHTML = data.author_name;
        document.getElementById("authorA").href = "/u/" + user_id;
        if (data.reference != "") {
            document.getElementById("referenceB").onclick = (() => window.open(data.reference, '_blank').focus());
            document.getElementById("referenceB").hidden = false;
        }

        if (data.project != "") {
            const temp = document.createElement("a");
            temp.download = data.name;
            temp.href = data.project;
            document.getElementById("fileB").onclick = (() => temp.click());
            document.getElementById("fileB").hidden = false;
        }

        for (let comment of data.commentaries) {
            const block = document.createElement("div");
            const name = document.createElement("a");
            name.href = "/u/" + comment.user_id;
            name.innerHTML = comment.name;
            block.appendChild(name);
            const image = new Image();
            image.src = comment.photo;
            block.appendChild(image);
            const description = document.createElement("label");
            description.innerHTML = comment.description;
            block.appendChild(description);
            document.getElementById("commentaries").appendChild(block);
        }
    }
});