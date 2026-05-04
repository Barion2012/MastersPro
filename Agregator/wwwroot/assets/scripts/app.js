
(function () {
    'use strict';

//App Settings
    DevExpress.localization.locale('ru');


//Signal
const hubConnection = typeof(signalR) === "undefined"? null : new signalR.HubConnectionBuilder()
     .withUrl("/qlab")
    .configureLogging(signalR.LogLevel.Error)
    .withAutomaticReconnect()
     .build();


if (typeof(signalR) !== "undefined") 
{



	hubConnection.on("Alert", (msg) => {
        	DevExpress.ui.notify({
			message: msg,
	            position: {
        	        my: "center top",
                	at: "center top"
			            }
	        }, "success", 10000)
	    });

	hubConnection.on("Receive", (userName, message) => {

	    // создаем элемент <b> для имени пользователя
	    let userNameElem = document.createElement("b");
	    userNameElem.appendChild(document.createTextNode(userName + ": "));

	    // создает элемент <p> для сообщения пользователя
	    let elem = document.createElement("p");
	    elem.appendChild(userNameElem);
	    elem.appendChild(document.createTextNode(message));

	    var firstElem = document.getElementById("chatroom").firstChild;
	    document.getElementById("chatroom").insertBefore(elem, firstElem);
	});


    hubConnection.on("ReceiveEx", (message) => {
       
        // создаем элемент <b> для имени пользователя
        //let userNameElem = document.createElement("b");
        //userNameElem.appendChild(document.createTextNode(message.user +'('+ message.who.toString() + "): "));

        // создает элемент <p> для сообщения пользователя
        let elem = document.createElement("div");
        
        //elem.appendChild(userNameElem);
        //elem.appendChild(document.createTextNode(message.text));
        

        if(message.who === 0)
        {
            elem.classList.add("align-self-end");
            elem.innerHTML = "<div class=\"chat-box-wrapper chat-box-wrapper-right\">\
                                <div><div class=\"chat-box\">"+message.text+"</div></div>\
                                <div class=\"avatar-icon-wrapper\">\
                                    <div class=\"avatar-icon avatar-icon-lg rounded\">\
                                        <img src=\"/User/Avatar\" alt=\"\">\
                                    </div>\
                                </div>\
                              </div>";
        }
        else if(message.who === 3)
        {
            elem.classList.add("chat-box-wrappert");
            elem.innerHTML = "<div class=\"chat-box-wrapper chat-box-wrapper-left\">\
                                <div class=\"avatar-icon-wrapper\">\
                                    <div class=\"avatar-icon avatar-icon-lg rounded\">\
                                        <img src=\"/assets/images/bot-icon.png\" alt=\"\">\
                                    </div>\
                                </div>\
                                <div><div class=\"chat-box chat-box-robot\">"+message.text+"</div></div>\
                              </div>"; 
        }
        else 
        {
            elem.classList.add("chat-box-wrappert");
            elem.innerHTML = "<div class=\"chat-box-wrapper chat-box-wrapper-left\">\
                                <div><div class=\"mr-2\">"+message.user+"</div></div>\
                                <div><div class=\"chat-box\">"+message.text+"</div></div>\
                              </div>";
        }
        //document.getElementById("chatroom").innerHTML+= content;
           
        document.getElementById("chatroom").appendChild(elem);
        document.getElementById("chatroom").scrollTop = document.getElementById("chatroom").scrollHeight;
    });



	hubConnection.start().then(() => hubConnection.invoke("Register"));
}





// Custom Code
// -----------------------------------


    $(initApp);

    function initApp() {


        $("#sendBtn").on('click', (e) => {
            var f = $("#message");
            hubConnection.invoke("SendEx", f.val());
            f.val('');
        });

        $(".submit_external").on('click', (e) => {
            $("input[name='ProviderName']").val($(e.target).attr('value'));
            document.forms['external-login-form'].submit();
        });
/*
        $("button#signContract").on('click', (e) => {
            console.log("button#signContract");
            $('#signCode2 > input')[0].focus();
            return false;
        });
        */



        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, (form) => {
            form.addEventListener('submit', (event) => {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });

        var toServerLog = (msg) => {
            fetch('/api/test?msg=' + msg,
                {
                    method: 'get', credentials: 'include',
                    headers: { 'Accept': 'text/html' }
                }).then(r => r.ok);
        };

     //   toServerLog('appInit');

        var video = $('#video');
        if (!!video[0] && navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

            //            console.log(navigator.mediaDevices);
            var width = 320;    // We will scale the photo width to this
            var height = 0;     // This will be computed based on the input stream
            var streaming = false;

            
            var canvas = $('#photo-canvas')[0];
            var context = canvas.getContext('2d');


            video.on('canplay', (ev) => {
                if (!streaming) {
                    /*
                    console.log(ev);
                    console.log(ev.target.videoHeight);
                    console.log(ev.target.videoWidth);
                    */
                    if (ev.target.videoHeight > ev.target.videoWidth) width = 240;

                    height = ev.target.videoHeight / (ev.target.videoWidth / width);

                    // Firefox currently has a bug where the height can't be read from
                    // the video, so we will make assumptions if this happens.

                    if (isNaN(height)) {
                        height = width / (4 / 3);
                    }

                //    toServerLog('videoHeight=' + ev.target.videoHeight.toString() + ';videoWidth=' + ev.target.videoWidth.toString())
                //    toServerLog('height=' + height.toString() + ';width=' + width.toString());

                    video.attr('width', width);
                    video.attr('height', height);
                    canvas.setAttribute('width', width);
                    canvas.setAttribute('height', height);
                    streaming = true;
                }
            });



            $('#make-selfportrait').on('click', (e) => {

                setTimeout(() => {

                    navigator.mediaDevices.getUserMedia({ video: true, audio: false }).then((stream) => {

                        $('#allow').css('display', 'none');

                        video[0].srcObject = stream;
                        video[0].play();
                    })
                        .catch((err) => {
                            console.log("An error occurred: " + err);
                        });
                }, 100)
            });

            $('#snap-selfportrait').on('click', (e) => {

                var sx = 0, sy = 0, sw = width, sh = height;

                if (width && height) {

                    if (width > height) {
                        //                        sx = (width - height) 

                        sx = height;
                        sy = height / 2;

                        sw = height;
                        sh = height;

                    }
                    else {

                        //sy = (height - width) / 2;

                        sx = width / 2;
                        sy = width;

                        sw = width;
                        sh = width;
                    }



                    //                context.translate(canvas.width, 0);
                    //               context.scale(-2, 2);
                    //                context.drawImage(video, 0, 0, 240, 240);
                    //                var base64dataUrl = canvas.toDataURL('image/jpeg');
                    //                context.setTransform(1, 0, 0, 1, 0, 0); // убираем все кастомные трансформации canvas
                    // на этом этапе можно спокойно отправить  base64dataUrl на сервер и сохранить его там как файл (ну или типа того)
                    /*
                            // но мы добавим эти тестовые снимки в наш пример:
                            var img = new Image();
                            img.src = base64dataUrl;
                            window.document.body.appendChild(img);
                            */
                    canvas.width = sw;
                    canvas.height = sh;
                    context.translate(canvas.width, 0);
                    context.scale(-1, 1);
                    context.drawImage(video[0], sx, sy, width, height, 0, 0, width, height);

                    toServerLog('sx:' + sx.toString()
                        + ',sy:' + sy.toString()
                        + ',swidth:' + width.toString()
                        + ',sheight:' + height.toString()
                        + ',dx:0,dy:0'
                        + ',width:' + width.toString()
                        + ',height:' + height.toString()

                    );

                    $('#stop-selfportrait').attr('class', 'btn btn-secondary');

                }
            });


            var stopcam = () => {
                video[0].pause();
                video[0].src = ""
                video[0].srcObject.getTracks()[0].stop();

            }

            $(".close").on('click', (e) => { if (streaming) stopcam() });

            $('#stop-selfportrait').on('click', (e) => {

                

                if (streaming && !$(e.target).hasClass('disabled')) {

                    stopcam();

                    fetch('/api/setphoto',
                        {
                            method: 'post', credentials: 'include',
                            headers: {
                                'Content-Type': 'application/json', 'Accept': 'text/html'
                            },
                            dataType: 'json',
                            body: JSON.stringify({
                                mimeType: 'image/jpeg',
                                data: canvas.toDataURL('image/jpeg')
                            })
                        })
                        .then(r => r.json())
                        .then(r => $('#my-photo').attr('src', r.data))
                }

            });


        }


        $(".nav-item").on('click', (e) => {

            var loc = $(e.target.parentElement).attr("uluru");
            if (loc !== null) {
                var uluru = JSON.parse(loc);
                var tmap = $(e.target.parentElement).attr("map");
                var marker = new google.maps.Marker({
                    position: uluru, map: new google.maps.Map(
                        document.getElementById(tmap), { zoom: 15, center: uluru })
                });
            }
            
        });


        $(".modal-selfportrait").on('display', (e)=> {
            console.log("display has changed to :" + $(this).attr('style'));
        });

        $("button#fideback-btn").on("click", (e) => {
            fetch('/api/onfideback',
                {
                    method: 'post'
                    , credentials: 'include'
                    , headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        step: "Request"
                        , Vacancy: $(e.target.parentElement).attr("vacancy")
                        , Message: "I here"
                    })
                }
            )
                .then(r => {
                    if (r.status===200) {
                        e.target.innerText='На рассмотрении';
                        $(e.target).attr('class', 'btn disabled btn-secondary');
                    }
                })
        });

        $("#verify-phone").on("click", (e) => {

            console.log(e);
            var phone = '+7' + $("#PhoneNumber>input").val();
            console.log(phone);
            $("#dial-phone").text(phone);
            console.log($("#dial-phone"));

            fetch('/api/verifyphone',
                {
                    method: 'post'
                    , credentials: 'include'
                    , headers: { 'Accept': 'text/html' }
                    , body: JSON.stringify({ phone: phone })
                })
           
                .then(r => {
                    if (r.status !== 200) {
                        $("#invalidCode").text('При отправке контрольного кода произошла ошибка. Проверьте правильность номера телефона и повторите попытку.');
                    }
                });
        });

        $('#verfied').on('click', (e) => {
            fetch('/api/confirmphone',
                {
                    method: 'post'
                    , credentials: 'include'
                    , headers: { 'Accept': 'text/html' }
                    , body: JSON.stringify({ phone: '+7' + $("#PhoneNumber > input").val(), code : $('#VerificationCode').val() })
                })
                .then(r => {
                    if (r.status === 200) {
                        var e = $("#verify-phone");
                        e[0].innerText = 'Номер потвержден';
                        e.attr('class', 'btn disabled btn-success');
                    }
                });
        });

        $("#Label_Female").on("click", (e) => {
            $("#Female").val('on');
            $("#Male").val('off');
        });

        $("#Label_Male").on("click", (e) => {
            $("#Male").attr('value', 'on');
            $("#Female").attr('value', 'off');
        });
    }

})();