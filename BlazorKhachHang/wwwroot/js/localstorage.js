window.saveProfileToLocalStorage = function (profile) {
    localStorage.setItem("profileName", profile.name);
    localStorage.setItem("profileEmail", profile.email);
    localStorage.setItem("profilePhone", profile.phone);
    localStorage.setItem("profileGender", profile.gender);
    localStorage.setItem("profileBirthday", profile.birthday);
};

window.loadProfileFromLocalStorage = function () {
    return {
        name: localStorage.getItem("profileName") || "",
        email: localStorage.getItem("profileEmail") || "",
        phone: localStorage.getItem("profilePhone") || "",
        gender: localStorage.getItem("profileGender") || "",
        birthday: localStorage.getItem("profileBirthday") || ""
    };
};

window.fetchHtmlContent = async function (url) {
    try {
        const response = await fetch(url);
        return await response.text();
    } catch {
        return "<p>Không thể tải nội dung.</p>";
    }
};
window.logout = function () {
    localStorage.removeItem("profileName");
    localStorage.removeItem("profileEmail");
    localStorage.removeItem("profilePhone");
    localStorage.removeItem("profileGender");
    localStorage.removeItem("profileBirthday");

    // nếu có lưu id đăng nhập
    localStorage.removeItem("khachHangId");
    localStorage.removeItem("username");

    window.location.href = "/login";
};
