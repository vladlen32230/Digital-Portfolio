document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
const validateEmail = (email) => {
    return String(email)
        .toLowerCase()
        .match(
            /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
        );
};

document.getElementById("registerB").onclick = (() => {
    const statusL = document.getElementById("statusL");
    const nameI = document.getElementById("nameI");
    const emailI = document.getElementById("emailI");
    const passwordI = document.getElementById("passwordI");
    const password_repeatI = document.getElementById("password_repeatI");
    if (nameI.value.length < 4 || nameI.value.length > 20) {
        statusL.innerHTML = "Имя должно быть от 4 до 20 символов";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (!validateEmail(emailI.value) || emailI.value.len > 50) {
        statusL.innerHTML = "Почта должна быть корректной и до 50 символов";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (passwordI.value.length < 6 || passwordI.value.length > 50) {
        statusL.innerHTML = "Пароль должен быть от 6 до 50 символов";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else if (passwordI.value != password_repeatI.value) {
        statusL.innerHTML = "Пароли должны совпадать";
        setTimeout(() => statusL.innerHTML = "", 3000);
    }

    else {
        const data = new FormData();
        data.append("Password", passwordI.value);
        data.append("Email", emailI.value);
        data.append("Name", nameI.value);
        fetch("api/registeruser", {
            method: "POST",
            body: data
        }).then(async (response) => {
            if (!response.ok) {
                statusL.innerHTML = await response.text();
                setTimeout(() => statusL.innerHTML = "", 3000);
            }

            else
                window.location.href = "/";
        })
    }
});