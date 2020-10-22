let timer = null;

//window.onscroll = () => {
//    clearTimeout(timer);
//    timer = setTimeout(() => {
//        console.log(window.scrollY);
//        if (window.scrollY > window.innerHeight * 0.26) {
//            document.querySelector('.navbar').classList.add('navbar-light');
//            console.log(window.scrollY);
//        } else {
//            document.querySelector('.navbar').classList.remove('navbar-light');
//        }
//    }, 100);

//};

document.onreadystatechange = () => {
    window.onscroll = () => {
        clearTimeout(timer);
        timer = setTimeout(() => {
            if (window.innerWidth > 768) {
                if (window.scrollY > window.innerHeight * 0.2) {
                    document.querySelector('.back-to-top').classList.remove('hidden');
                } else {
                    console.log(window.scrollY);
                    document.querySelector('.back-to-top').classList.add('hidden');
                }
            }
        }, 500);

    };
}
