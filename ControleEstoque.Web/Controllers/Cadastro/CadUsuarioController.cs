﻿using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class CadUsuarioController : Controller
    {       
        // Abre a view com a lista acima, caso queira usar a lista.
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.SenhaPadrao = VarGlobal.SenhaPadrao;
            ViewBag.PaginaAtual = VarGlobal.PaginaAtual;
            var lista = UsuarioModel.RecuperarLista(ViewBag.PaginaAtual);
            var Quant = UsuarioModel.RecuperarQuantidade();

            var difQuantPaginas = (Quant % ViewBag.QuantidadeMaxLinhaPorPagina) > 0 ? 1 : 0; //Se tiver resto joga 1
            ViewBag.QuantPaginas = difQuantPaginas + (lista.Count / ViewBag.QuantidadeMaxLinhaPorPagina); //Divide a qtde de registro somando o resto.

            return View(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult UsuarioPagina(int pagina)
        {
            var lista = UsuarioModel.RecuperarLista(pagina);
            return Json(lista);
        }

        //Recupera o registro para confirmar ou excluir
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarUsuario(int id)
        {
            return Json(UsuarioModel.RecuperarPeloId(id));
        }

        //Passa o carro no registro
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(int id)
        {
            return Json(UsuarioModel.ExcluirPeloId(id));
        }

        //Salva adicionando o item na lista
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUsuario(UsuarioModel model)
        {
            string msg = GlobalMsg.RetornaMsg((int)GlobalMsg.OptionErro.OK);
            var resultado = msg;
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            //Validação
            if (!ModelState.IsValid)
            {
                msg = GlobalMsg.RetornaMsg((int)GlobalMsg.OptionErro.AVISO);
                resultado = msg;

                //lista de mensagem
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    if (model.Senha == VarGlobal.SenhaPadrao)
                    {
                        model.Senha = "";
                    }

                    //Busca o Registro
                    var id = model.Salvar();

                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                    else
                    {
                        msg = GlobalMsg.RetornaMsg((int)GlobalMsg.OptionErro.ERRO);
                        resultado = msg;
                    }

                }
                catch (Exception ex)
                {
                    msg = GlobalMsg.RetornaMsg((int)GlobalMsg.OptionErro.ERRO);
                    resultado = msg;
                }
            }
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }

    }
}