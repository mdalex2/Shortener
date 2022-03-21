var dataTable;
$("#btnNew").click(function () {
   
    $("#ventana_modal_body").load(urlUpsert, function () {
        $("#ventana_modal").modal("show");
        $("#ventana_modal_title").text("Acortar URL");
    });
});

$("#btnFiltrar").click(function () {
   
    $(dataTable).DataTable().destroy();
    
    loadDataTable();
});

$("#btnLimpiarFiltro").click(function () {
    $("#txtFiltro").val("");
    $(dataTable).DataTable().destroy();
    loadDataTable();
});

//$("#btnSave").click(function () {
//    alert("Clic");
   
//});

function Edit(urlUpsert)
{    
    $("#ventana_modal_body").load(urlUpsert, function () {
        $("#ventana_modal").modal("show");
        $("#ventana_modal_title").text("Acortar URL");
    });
}

//function Save()
//{
//    var datos = $('#frmDatos').serialize();
//    var urlUpsert = "/Acortador/Upsert";
//    debugger;
//    $.ajax({
//        url: urlUpsert,
//        data: { datos },
//        type: 'POST',
//        dataType: 'json',
//        success: function (json) {
//            $('<h1/>').text(json.title).appendTo('body');
//            $('<div class="content"/>')
//                .html(json.html).appendTo('body');
//        },
//        error: function (xhr, status) {
//            alert('Disculpe, existió un problema');
//        },
//        complete: function (xhr, status) {
//            alert('Petición realizada');
//        }
//    });
//}


$(document).ready(function ()
{
    loadDataTable();    
});

function loadDataTable() {
    let filtro = $("#txtFiltro").val();
    debugger;
    $("#tblDatos").dataTable().fnDestroy();
    dataTable = $("#tblDatos").DataTable({
        "ajax": {
            "url": "/Acortador/ObtenerTodos/",
            data: {"filtro": filtro},
            "error": function (error) {
                // Message also does not show here
                console.log(error);
            }
        },
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        "responsive": true,
        "ordering": false,
        "language": {
            "url": "/lib/datatables/es_es.json"
        },
        "columns": [
            { "data": "codProducto", "width": "1%" },
            { "data": "urlLarga", "width": "10%" },
            
            {
                "data": "urlCorta",
                "render": function (data) {
                    let url = serverUrl + data;                    
                    return `<a href="${url}" target="_blank">${url}</a>`;
                },
                "width": "0%"
            },
            //{ "data": "urlCorta", "width": "0%" },
            {
                "data": "fechaModificacion",
                "render": function (data) {
                    let fecha =moment(data).format("DD/MM/YYYY HH:mm");
                    return fecha;
                },
                "width": "0%"
            },
            {
                "data": "fechaExpira",
                "render": function (data) {
                    if (data == null) return "-";
                    let fecha = moment(data).format("DD/MM/YYYY HH:mm");
                    return fecha;
                },
                "width": "0%"
            },
            {
                "data": "habilitado",
                "render": function (data) {
                    let estado = (data == true) ? "Activo" : "Inactivo";
                    return `${estado}`;
                },
                "width": "0%"
            },
            { "data": "numVisitas", "width": "0%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="#" onclick=Edit("/Acortador/Upsert/${data}") class="btn bg-gradient btn-primary text-white"><i class="fas fa-edit"></i></a>
                            <a href="#" onclick=Delete("/Acortador/Delete/${data}") class="btn bg-gradient btn-danger text-white"><i class="fas fa-trash-alt"></i></a>
                        </div>
                    `;
                }, "width": "1%"
            }
        ]
    });
}

function Delete(url) {
    new swal({
        title: "¿Desea eliminar la url?",
        text: "Este registro no se podrá recuperar",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Borrar',
        cancelButtonText: 'Cancelar',
        dangermode: true
    }).then((borrar) => {
        if (borrar.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}



