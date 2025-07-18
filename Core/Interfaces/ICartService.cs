﻿using Core.Models.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface ICartService
{
    Task CreateUpdate(CartCreateUpdateModel model);
    Task<List<CartItemModel>> GetCartItems();
    Task Delete(long id);

}
