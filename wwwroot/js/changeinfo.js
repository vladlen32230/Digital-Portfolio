const user_id = window.location.pathname.split("/")[2];
document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
fetch("/api/u/" + user_id + "/getdescription", {
    method: "GET"
}).then(async (response) => {
    if (response.ok) {
        const data = await response.json();
        document.getElementById("descriptionI").value = data.description;
        document.getElementById("first_skillI").value = data.first_skill;
        document.getElementById("second_skillI").value = data.second_skill;
        document.getElementById("third_skillI").value = data.third_skill;
        document.getElementById("vkI").value = data.vk;
        document.getElementById("telegramI").value = data.telegram;
    }
});

document.getElementById("sendB").onclick = (() => {
    const statusL = document.getElementById("statusL");
    const descriptionI = document.getElementById("descriptionI");
    const first_skillI = document.getElementById("first_skillI");
    const second_skillI = document.getElementById("second_skillI");
    const third_skillI = document.getElementById("third_skillI");
    const vkI = document.getElementById("vkI");
    const telegramI = document.getElementById("telegramI");
    if (descriptionI.value.length >= 2500) {
        statusL.innerHTML = "����� �������� ������ ���� �� ����� 2500 ��������";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (first_skillI.value.length >= 50 || second_skillI.value.length >= 50 || third_skillI.value.length >= 50) {
        statusL.innerHTML = "����� �������� ������� ������ ���� �� ����� 50 ��������";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (vkI.value.length >= 64
        || telegramI.value.length >= 64) {
        statusL.innerHTML = "����� ������ ������ ���� �� ����� 64 ��������";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else {
        const data = new FormData();
        const photoI = document.getElementById("photoI");
        data.append("description", descriptionI.value);
        data.append("first_skill", first_skillI.value);
        data.append("second_skill", second_skillI.value);
        data.append("third_skill", third_skillI.value);
        data.append("vk", vkI.value);
        data.append("telegram", telegramI.value);
        const reader = new FileReader();
        const file = photoI.files[0];
        if (file != null) {
            reader.readAsDataURL(file);
            reader.onload = (async () => {
                const photo = await resizeImage(reader.result, 400, 400);
                data.append("photo", photo);
                send(data);
            });
        }

        else
            send(data);
    }
});

function send(form) {
    fetch("/api/u/" + user_id + "/saveinfo", {
        method: "PUT",
        body: form
    }).then((response) => {
        if (response.ok)
            window.location.href = "/u/" + user_id;
    });
}

const resizeImage = (base64Str, maxWidth, maxHeight) => {
    return new Promise((resolve) => {
        let img = new Image()
        img.src = base64Str
        img.onload = () => {
            let canvas = document.createElement('canvas')
            const MAX_WIDTH = maxWidth
            const MAX_HEIGHT = maxHeight
            let width = img.width
            let height = img.height

            if (width > height) {
                if (width > MAX_WIDTH) {
                    height *= MAX_WIDTH / width
                    width = MAX_WIDTH
                }
            } else {
                if (height > MAX_HEIGHT) {
                    width *= MAX_HEIGHT / height
                    height = MAX_HEIGHT
                }
            }
            canvas.width = width
            canvas.height = height
            let ctx = canvas.getContext('2d')
            ctx.drawImage(img, 0, 0, width, height)
            resolve(canvas.toDataURL())
        }
    })
}