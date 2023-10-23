using Microsoft.AspNetCore.Mvc;
using Teste_MStar.Context;
using Teste_MStar.Models;

namespace Teste_MStar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly MStarContext _context;

        public ProductController(MStarContext context)
        {
            _context = context;
        }

        [HttpPost("CreateProduct")]
        public ActionResult CreateProduto(Products product)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.NumeroRegistro == product.NumeroRegistro);
                if (existingProduct != null)
                    return BadRequest("Já existe um produto com o mesmo número de registro.");

                _context.Add(product);
                _context.SaveChanges();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
            }
        }

        [HttpGet("GetAllProducts")]
        public ActionResult GetAllProducts()
        {
            var productAll = _context.Products.ToList();
            return Ok(productAll);
        }

        [HttpGet("GetProductNumeroRegistro/{nrRegistro}")]
        public ActionResult GetProductsNumeroSerie(string nrRegistro)
        {
            try
            {
                var productExist = _context.Products.FirstOrDefault(x => x.NumeroRegistro.ToString() == nrRegistro);
                return Ok(productExist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
            }
        }

        [HttpPut("ProductIn/{nrRegistro}")]
        public ActionResult ProductIn(string nrRegistro, [FromBody] InPutLog inPutLog)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var productExist = _context.Products.FirstOrDefault(x => x.NumeroRegistro.ToString() == nrRegistro);

                    if (productExist == null)
                        return NotFound("Produto não encontrado.");

                    var existingInPutLog = _context.InPutLog.FirstOrDefault(x => x.NumeroRegistro.ToString() == nrRegistro);
                    if (existingInPutLog == null)
                        return NotFound("Registro InPutLog não encontrado.");

                    existingInPutLog.Local = inPutLog.Local;
                    existingInPutLog.QtdProduct = inPutLog.QtdProduct;

                    productExist.QtdProduct += inPutLog.QtdProduct;

                    _context.SaveChanges();
                    transaction.Commit();

                    return Ok("Registro de entrada realizado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
            }
        }

        [HttpPut("ProductOut/{nrRegistro}")]
        public ActionResult ProductOut(string nrRegistro, [FromBody] OutPutLog outPutLog)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.NumeroRegistro.ToString() == nrRegistro);

                if (product == null)
                    return NotFound("Produto não encontrado.");

                if (product.QtdProduct < outPutLog.QtdProduct)
                    return BadRequest("Quantidade de produto insuficiente.");

                product.QtdProduct -= outPutLog.QtdProduct;

                _context.Add(outPutLog);
                outPutLog.Date = DateTime.Now;
                _context.SaveChanges();

                return Ok("Registro de saída realizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
            }
        }
    }
}