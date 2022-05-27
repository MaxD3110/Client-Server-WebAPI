using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using System;
using System.ComponentModel;
using System.Linq;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {
        private readonly string PATH = $"{Environment.CurrentDirectory}\\cardDataList.json";
        private BindingList<CardModel> _cardDataList;
        private FileIOService _fileIOService;


        [HttpGet]
        public BindingList<CardModel> Get()
        {
            _fileIOService = new FileIOService(PATH);

            try
            {
                _cardDataList = _fileIOService.LoadData();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _cardDataList;
        }

        [HttpPut]
        public void Put(BindingList<CardModel> card)
        {
            _fileIOService = new FileIOService(PATH);

            var sorted = new BindingList<CardModel>(card.OrderBy(c => c.Name).ToList());

            for (int i = 0; i < sorted.Count(); i++)
            {
                sorted[i].Id = i;
            }

            try
            {
                _fileIOService.SaveData(sorted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("{Name}")]
        public void Post(string Name, [FromBody] string Photo)
        {
            _fileIOService = new FileIOService(PATH);
            _cardDataList = _fileIOService.LoadData();

            CardModel card = new CardModel() { Id = _cardDataList.Count(), Name = Name, Photo = Photo };

            _cardDataList.Add(card);

            Put(_cardDataList);
        }

        [HttpPost]
        public void Post(int[] Ids)
        {
            _fileIOService = new FileIOService(PATH);
            _cardDataList = _fileIOService.LoadData();

            for (int i = 0; i < Ids.Length; i++)
            {
                CardModel card = _cardDataList.SingleOrDefault(c => c.Id == Ids[i]);
                _cardDataList.Remove(card);
            }

            Put(_cardDataList);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _fileIOService = new FileIOService(PATH);
            _cardDataList = _fileIOService.LoadData();

            CardModel card = _cardDataList.SingleOrDefault(c => c.Id == id);
            _cardDataList.Remove(card);

            Put(_cardDataList);
        }
    }
}
