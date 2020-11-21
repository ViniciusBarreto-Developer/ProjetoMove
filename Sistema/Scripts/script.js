document.addEventListener('DOMContentLoaded', () => {
    backToTop();
    modalBehavior();
    validateEmail();
    editBio();
});

function backToTop() {
    let timer = null;
    window.onscroll = () => {
        clearTimeout(timer);
        timer = setTimeout(() => {
            if (window.innerWidth > 768) {
                if (window.scrollY > window.innerHeight * 0.2) {
                    document.querySelector('.back-to-top').classList.remove('hidden');
                } else {
                    document.querySelector('.back-to-top').classList.add('hidden');
                }
            }
        }, 500);
    };
}

function modalBehavior() {
    const openModalBtns = document.querySelectorAll('[data-open-modal]');

    if (openModalBtns) {
        const closeModalBtns = document.querySelectorAll('[data-close-modal]');
        const allModals = document.querySelectorAll('.modal-bg');

        openModalBtns.forEach((item) => {
            item.addEventListener('click', openModal(item.dataset.openModal));
        });

        closeModalBtns.forEach((item) => {
            item.addEventListener('click', closeModal(item.dataset.closeModal));
        });

        allModals.forEach((item) => {
            item.addEventListener('click', (e) => {
                outsideClick(e, item)
            });
        });

        function openModal(targetId) {
            return () => {
                const modalBg = document.querySelector(`#${targetId}`);
                const modal = document.querySelector(`#${targetId}`).firstElementChild

                document.body.style.overflow = 'hidden';
                modalBg.classList.remove('hidden');
                modal.classList.remove('scale-out-center');
                modal.classList.add('scale-in-center');
            }
        }

        function closeModal(targetId) {
            return () => {
                const modalBg = document.querySelector(`#${targetId}`);
                const modal = document.querySelector(`#${targetId}`).firstElementChild

                document.body.style.overflow = 'auto';
                modal.classList.add('scale-out-center');;
                modal.classList.remove('scale-in-center');
                setTimeout(() => {
                    modalBg.classList.add('hidden');
                }, 200)

            }
        }

        function outsideClick(e, modalBg) {
            if (e.target === modalBg) {
                const modal = document.querySelector('.modal:not(hidden)')

                document.body.style.overflow = 'auto';
                modal.classList.add('scale-out-center');;
                modal.classList.remove('scale-in-center');
                setTimeout(() => {
                    modalBg.classList.add('hidden');
                }, 200)
            }
        }
    }
}

function editBio() {
    const bioTxt = document.querySelector('#editTxt');
    if (bioTxt) {
        //AJAX edit clicando fora
        //bioTxt.addEventListener('focusout', () => {
        //    const data = {
        //        bio: bioTxt.value
        //    }

        //    fetch("/Home/EditarBiografia", {
        //        method: 'POST',
        //        headers: headers,
        //        body: JSON.stringify(data)
        //    })
        //})

        //JEITO ANTIGO
        const editBioBtn = document.querySelector('#editBio');
        const confirmBioBtn = document.querySelector('#confirmBio');

        editBioBtn.addEventListener('click', () => {
            if (bioTxt.hasAttribute('readonly')) {
                allowEdit();
            }
            else {
                blockEdit();
            }
        });

        confirmBioBtn.addEventListener('click', () => {
            if (bioTxt.hasAttribute('readonly')) {
                allowEdit();
            }
            else {
                blockEdit();
            }
        });

        function allowEdit() {
            bioTxt.toggleAttribute('readonly');
            bioTxt.focus();
            toggleBtnEdit();
        }
        function blockEdit() {
            bioTxt.toggleAttribute('readonly');
            toggleBtnEdit();
        }
        function toggleBtnEdit() {
            editBioBtn.classList.toggle('hidden-animated');
            confirmBioBtn.classList.toggle('hidden-animated');
        }
    }
}

function validateEmail() {
    const emailInput = document.querySelector('#Email');
    if (emailInput) {
        const msg = document.querySelector('#msg');
        const button = document.querySelector('#concluir');
        const headers = {
            "Content-Type": "application/json",
            "Access-Control-Origin": "*"
        }
        emailInput.addEventListener('focusout', () => {

            const data = {
                email: emailInput.value
            }

            fetch("/Home/ValidarEmail", {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(data)
            })
                .then((res) => res.json())
                .then((resJson) => {
                    if (resJson === "s") {
                        msg.innerText = "E-mail já cadastrado";
                        msg.classList.add("text-danger", "field-validation-error")
                        button.setAttribute("disabled", "disabled");
                    }
                    else {
                        button.removeAttribute("disabled");
                    }
                });
        });
    }
}

function validateEmailRecuperacao() {
    const emailInput = document.querySelector('#EmailRecuperacao');
    if (emailInput) {
        const msg = document.querySelector('#msgRec');
        const button = document.querySelector('#concluir');
        const headers = {
            "Content-Type": "application/json",
            "Access-Control-Origin": "*"
        }
        emailInput.addEventListener('focusout', () => {

            const data = {
                email: emailInput.value
            }

            fetch("/Home/ValidarEmailRecuperacao", {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(data)
            })
                .then((res) => res.json())
                .then((resJson) => {
                    if (resJson === "s") {
                        msg.innerText = "E-mail já utilizado para recuperação";
                        msg.classList.add("text-danger", "field-validation-error")
                        button.setAttribute("disabled", "disabled");
                    }
                    else {
                        button.removeAttribute("disabled");
                    }
                });
        });
    }
}

//function imgUploadPreview() {
//    const imgInput = document.querySelector("#img-input");

//    if (imgInput) {
//        //const previewContainer = document.querySelector("#preview-container");
//        const preview = document.querySelector("#img-preview");
//        if (preview.src === "") {
//            preview.classList.add("hidden");
//        }

//        imgInput.addEventListener("change", (e) => {
//            preview.classList.remove("hidden");
//            preview.src = URL.createObjectURL(e.target.files[0]);
//            preview.onload = () => URL.revokeObjectURL(preview.src)
//        })
//    }
//}
