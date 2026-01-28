@model ProjReceitas.Models.DetalhesReceitaViewModel


var porcentagem = @Model.MediaAvaliacoes; //numero de 0 a 5;

const porc = document.querySelector(".porcentagem");
const exibir = document.querySelector(".exibir-porcentagem");

iniciar();

function iniciar() {
    let val = porcentagem * 20;

    exibir.innerText = `${val}%`;
    porc.style.width = `${val}%`;
}