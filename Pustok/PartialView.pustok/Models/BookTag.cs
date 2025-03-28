﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PartialView.pustok.Models
{
    public class BookTag :BaseEntity
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
