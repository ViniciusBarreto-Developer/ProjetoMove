document.addEventListener('DOMContentLoaded', () => {
    backToTop();
    modalBehavior();
    editBio();
    validateEmail();
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
                document.body.style.overflow = 'hidden';
                document.querySelector(`#${targetId}`).classList.remove('hidden-animated');
            }
        }

        function closeModal(targetId) {
            return () => {
                document.body.style.overflow = 'visible';
                document.querySelector(`#${targetId}`).classList.add('hidden-animated');
            }
        }

        function outsideClick(e, modal) {
            if (e.target === modal) {
                modal.classList.add('hidden-animated');
                document.body.style.overflow = 'visible';
            }
        }
    }
}

function editBio() {
    const bioTxt = document.querySelector('#editTxt');
    if (bioTxt) {
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