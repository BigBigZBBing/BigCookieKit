﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Office.Xlsx
{
    class XlsxRowConfigException : Exception
    {
        public XlsxRowConfigException()
        {
        }

        public XlsxRowConfigException(string message) : base(message)
        {
        }
    }
}
