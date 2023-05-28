const user_id = window.location.pathname.split("/")[2];
const portfolio_id = window.location.pathname.split("/")[4];
document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
fetch("/api/u/" + user_id + "/p/" + portfolio_id + "/getinfo", {
    method: "GET"
}).then(async (response) => {
    if (response.ok) {
        const data = await response.json();
        document.getElementById("photoIm").setAttribute("src", data.photo);
        document.getElementById("nameL").innerHTML = data.name;
        document.getElementById("descriptionL").innerHTML = (data.description);
        document.getElementById("ratingL").innerHTML = data.rating;
        document.getElementById("rating_statusL").innerHTML = data.author_rate;
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
            if (comment.is_owner) {
                const deleteB = document.createElement("button");
                deleteB.innerHTML = "�������";
                deleteB.onclick = (() => {
                    fetch("/api/deletecomment/" + comment.user_id + "/" + comment.id, {
                        method: "GET"
                    }).then((response) => {
                        if (response.ok)
                            window.location.reload();
                    });
                });

                block.appendChild(deleteB);
            }

            document.getElementById("commentaries").appendChild(block);
        }
    }
});

document.getElementById("send_commentaryB").onclick = (() => {
    const data = new FormData();
    data.append("description", document.getElementById("commentaryI").value);
    fetch("/api/u/" + user_id + "/p/" + portfolio_id + "/addcomment", {
        method: "POST",
        body: data
    }).then((response) => {
        if (response.ok)
            window.location.reload();
    });
});

document.getElementById("likeB").onclick = (() => {
    fetch("/api/u/" + user_id + "/p/" + portfolio_id + "/like", {
        method: "POST"
    }).then(async (response) => {
        if (response.ok)
            document.getElementById("rating_statusL").innerHTML = await response.text();
    });
});

document.getElementById("dislikeB").onclick = (() => {
    fetch("/api/u/" + user_id + "/p/" + portfolio_id + "/dislike", {
        method: "POST"
    }).then(async (response) => {
        if (response.ok)
            document.getElementById("rating_statusL").innerHTML = await response.text();
    });
});