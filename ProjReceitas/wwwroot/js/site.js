var marginLateral;
var maginMax;
var deslocamento = 695;
var numEsq;
var numDir;

const setaEsq = document.querySelector(".esquerda");
const setaDir = document.querySelector(".direita");
const navegacao = document.querySelector(".navegacao");
const imagens = document.querySelector(".imagens");

const allCards = document.querySelectorAll('.card');

iniciar();

function iniciar() {

    imagens.style.justifyContent = "center";

    if (allCards.length % 2 != 0) {
        marginLateral = 0;

        if (allCards.length == 1) {
            numEsq = 0;
            numDir = 0;

        }

        else if (allCards.length != 1) {
            let num = allCards.length;

            maginMax = ((num - 1) / 2) * deslocamento;
            numEsq = maginMax;
            numDir = maginMax;
        }
    }

    else if (allCards.length % 2 == 0) {
        marginLateral = -342.5;
        navegacao.style.marginRight = `${marginLateral}px`;

        let num = allCards.length;

        numEsq = ((num / 2) - 1) * deslocamento;
        numDir = (num / 2) * deslocamento - 342.5;
    }

}

function setaDireita() {
    if (marginLateral >= numDir) {
        setaEsq.style.opacity = "0";
    }

    else {
        marginLateral += deslocamento;
        navegacao.style.marginRight = `${marginLateral}px`;
        setaDir.style.opacity = "1";
    }

}

function setaEsquerda() {
    if (marginLateral <= numEsq * -1) {
        setaDir.style.opacity = "0";
    }

    else {
        marginLateral -= deslocamento;
        navegacao.style.marginRight = `${marginLateral}px`;
        setaEsq.style.opacity = "1";
    }
}