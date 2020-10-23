let timer = null;

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

