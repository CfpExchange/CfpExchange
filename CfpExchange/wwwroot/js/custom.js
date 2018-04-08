function getMetadata(url) {
    $.ajax({
        type: "POST",
        url: "/cfp/getMetadata",
        data: {"url":url},
        success: function(metadata) {
            $('#eventUrlPreview a').attr('href', metadata.url);
            $('#eventUrlPreview img').attr('src', metadata.imageUrl);
            $('#eventUrlPreview h3').text(metadata.title);
            $('#eventUrlPreview p').html(metadata.description);eventTitle

            $('#eventTitle').attr('value', metadata.title);
            $('#eventUrlPreview').show();
        }
    });
}