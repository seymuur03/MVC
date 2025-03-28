$(document).ready(function () {
    $("#Photo").change(function (e) {
        let file = e.target.files[0];
        var uploading = new FileReader();
        uploading.onload = function (displaying) {
            $("#ShowImg").attr('src', displaying.target.result)
            $("#ShowImg").show();
        }
        uploading.readAsDataURL(file)
    })
})