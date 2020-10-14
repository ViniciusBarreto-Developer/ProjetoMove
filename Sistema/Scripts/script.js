let timer = null;

window.onscroll = () => {
    clearTimeout(timer);
    timer = setTimeout(() => {
        console.log(window.scrollY);
        if (window.scrollY > window.innerHeight * 0.26) {
            document.querySelector('.navbar').classList.add('navbar-light');
            console.log(window.scrollY);
        } else {
            document.querySelector('.navbar').classList.remove('navbar-light');
        }
    }, 100);
    
};
