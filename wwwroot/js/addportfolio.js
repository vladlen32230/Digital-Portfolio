document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
const user_id = window.location.pathname.split("/")[2];
document.getElementById("saveB").onclick = (() => {
    const statusL = document.getElementById("statusL");
    if (document.getElementById("nameI").value.length < 4 || document.getElementById("nameI").value.length > 40) {
        statusL.innerHTML = ("��� ������ ��������� �� 4 �� 40 ��������");
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (document.getElementById("descriptionI").value.length > 2500) {
        statusL.innerHTML = ("�������� ������ ��������� �� 2500 ��������");
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (document.getElementById("referenceI").value.length > 255) {
        statusL.innerHTML = ("������ ������ ��������� �� 255 ��������");
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else {
        const data = new FormData();
        data.append("name", document.getElementById("nameI").value);
        data.append("description", document.getElementById("descriptionI").value);
        data.append("reference", document.getElementById("referenceI").value);
        const reader_photo = new FileReader();
        const reader_file = new FileReader();
        const photo_file = document.getElementById("photoI").files[0];
        const file = document.getElementById("fileI").files[0];
        if (photo_file != null && file != null) {
            reader_photo.readAsDataURL(photo_file);
            reader_photo.onload = (async () => {
                const photo = await resizeImage(reader_photo.result, 400, 400);
                reader_file.readAsDataURL(file);
                reader_file.onload = (() => {
                    data.append("photo", photo);
                    data.append("file", reader_file.result);
                    send_data(data);
                })
            })
        }

        else if (photo_file != null) {
            reader_photo.readAsDataURL(photo_file);
            reader_photo.onload = (async () => {
                const photo = await resizeImage(reader_photo.result, 400, 400);
                data.append("photo", photo);
                send_data(data);
            });
        }

        else if (file != null) {
            reader_file.readAsDataURL(file);
            reader_file.onload = (() => {
                data.append("file", reader_file.result);
                send_data(data);
            });
        }

        else
            send_data(data);
    }
});

function send_data(form) {
    fetch("/api/u/" + user_id + "/addportfolio", {
        method: "POST",
        body: form
    }).then((response) => {
        if (response.ok)
            window.location.href = "/u/" + user_id;
        else {
            statusL.innerHTML = "��������� �������������� ������";
        }
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