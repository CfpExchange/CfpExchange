// Grill 1.0
$(document).ready(function() {

    $('.js-search-category, .js-search-category2').select2();


    $(".js-search-ingredients").select2({
        maximumSelectionLength: 4
    });

    $("#sortable").sortable();
    $("#sortable").disableSelection();


    $(".btn-light").click(function() {
        event.preventDefault();
        $("#sortable").append('<div class="box ui-sortable-handle">\
                    <div class="row">\
                      <div class="col-lg-1 col-sm-1">\
                        <i class="fa fa-arrows" aria-hidden="true"></i>\
                      </div>\
                      <div class="col-lg-5 col-sm-5">\
                        <input type="text" class="form-control" placeholder="Name of ingredient">\
                      </div>\
                      <div class="col-lg-5 col-sm-5">\
                        <input type="text" class="form-control" placeholder="Notes (quantity or additional info)">\
                      </div>\
                      <div class="col-lg-1 col-sm-1">\
                        <i class="fa fa-times-circle-o minusbtn" aria-hidden="true"></i>\
                      </div>\
                    </div>\
                  </div>');
    });

    $("#sortable").on("click", ".minusbtn", function() {
        $(this).parent().parent().parent().remove();
    });


});