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

    $("#MainPhoto").change(function (e) {
        let filee = e.target.files[0];
        var uploadingg = new FileReader();
        uploadingg.onload = function (displayingg) {
            $("#ShowImgg").attr('src', displayingg.target.result)
            $("#ShowImgg").show();
        }
        uploadingg.readAsDataURL(filee)
    })

    $("#HoverPhoto").change(function (e) {
        let file = e.target.files[0];
        var uploading = new FileReader();
        uploading.onload = function (displaying) {
            $("#ShowImg").attr('src', displaying.target.result)
            $("#ShowImg").show();
        }
        uploading.readAsDataURL(file)
    })
    

    $("#Photos").on("change", function () {
        var files_ = this.files;
        for (var i = 0; i < files_.length; i++) {
            let _file = files_[i]   
            if (_file) {    
                var uploadImg = new FileReader();
                uploadImg.onload = function (displayImg) {
                    let div = $("<div>");
                    let img = $("<img>").attr('src', displayImg.target.result).css('width', '100px')
                    $(div).append(img)
                    $("#ShowPhotoFrame").append(div)
                }
                uploadImg.readAsDataURL(_file)
            }

        }
    })
})