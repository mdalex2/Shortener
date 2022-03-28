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


function Edit(urlUpsert)
{    
    $("#ventana_modal_body").load(urlUpsert, function () {
        $("#ventana_modal").modal("show");
        $("#ventana_modal_title").text("Acortar URL");
    });
}

function VerQR(urlVerQR) {
    $("#ventana_modal_lg_body").load(urlVerQR, function () {
        $("#ventana_modal_lg").modal("show");
       
    });
}
function VerQRSvg(urlVerQR) {
    $("#ventana_modal_lg_body").load(urlVerQR, function () {
        $("#ventana_modal_lg").modal("show");
       
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
        "columnDefs": [
            { "visible": false, "targets": [0], "searchable": false },
        ],
        "columns": [
            { "data": "id", "width": "0%" },
            { "data": "codProducto", "width": "1%" },
            {
                "data": "urlCorta",
                "render": function (data) {
                    let url = serverUrl + data;
                    return `<a href="${url}" target="_blank" title="${url}"><div style="white-space: nowrap;  width:70px;  overflow: hidden;  text-overflow: ellipsis;">${data}</div></a>`;
                },
                "width": "1%"
            },
            {
                "data": "urlLarga",
                "render": function (data) {
                    return `<a href="${data}" target="_blank" title=${data}><div style="white-space: nowrap;  width:450px;  overflow: hidden;  text-overflow: ellipsis;">${data}</div></a>`;
                },
                "width": "5%"
            },            
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
                        <div class="row" style="width:160px;"><div class="text-center">
                            <a href="#" onclick=VerQRSvg("/Acortador/QrImagenSvg/${data}") class="btn bg-gradient btn-secondary"><i class="fas fa-qrcode" title="Ver QR"></i></a>
                            <a href="#" onclick=Edit("/Acortador/Upsert/${data}") class="btn bg-gradient btn-primary text-white"><i class="fas fa-edit"></i></a>
                            <a href="#" onclick=Delete("/Acortador/Delete/${data}") class="btn bg-gradient btn-danger text-white"><i class="fas fa-trash-alt"></i></a>
                        </div></div>
                    `;
                }, "width": "0%"
            }
        ]
    });

    $('table td').each(function () {
        var step = 5; $td = $(this);
        $td.width(10 + (step * $td.index()));
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
