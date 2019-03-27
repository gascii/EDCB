﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace EpgTimer.EpgView
{
    class EpgViewPanel : ViewPanel, IEpgSettingAccess, IEpgViewDataSet
    {
        protected EpgViewData viewData;
        public int EpgSettingIndex { get; private set; }
        public void SetViewData(EpgViewData data)
        {
            viewData = data;
            EpgSettingIndex = data.EpgSettingIndex;
            Background = this.EpgBrushCache().BackColor;
            SetBorderStyleFromSettings();
        }

        public override void SetBorderStyleFromSettings()
        {
            SetBorderStyle(this.EpgStyle().EpgBorderLeftSize, this.EpgStyle().EpgBorderTopSize, new Thickness(3, 0, 3, this.EpgStyle().FontSize * 0.8));
        }

        protected override void CreateDrawTextListMain(List<List<Tuple<Brush, GlyphRun>>> textDrawLists)
        {
            var ItemFontNormal = ItemFontCache.ItemFont(this.EpgStyle().FontName, false);
            var ItemFontTitle = ItemFontCache.ItemFont(this.EpgStyle().FontNameTitle, this.EpgStyle().FontBoldTitle);
            var DictionaryNormal = viewData.ReplaceDictionaryNormal;
            var DictionaryTitle = viewData.ReplaceDictionaryTitle;

            bool extraInfo = PopUpMode == true ? this.EpgStyle().EpgExtInfoPopup : this.EpgStyle().EpgExtInfoTable;

            double sizeTitle = this.EpgStyle().FontSizeTitle;
            double sizeMin = Math.Max(sizeTitle - 1, Math.Min(sizeTitle, this.EpgStyle().FontSize));
            double sizeNormal = this.EpgStyle().FontSize;
            double indentTitle = sizeMin * 1.7;
            double indentNormal = this.EpgStyle().EpgTitleIndent ? indentTitle : 0;
            Brush colorTitle = this.EpgBrushCache().TitleColor;
            Brush colorNormal = this.EpgBrushCache().NormalColor;

            foreach (ProgramViewItem info in Items)
            {
                var textDrawList = new List<Tuple<Brush, GlyphRun>>();
                textDrawLists.Add(textDrawList);
                Rect drawRect = TextRenderRect(info);
                info.TitleDrawErr = sizeTitle > drawRect.Height;

                //分
                string min = info.Data.StartTimeFlag == 0 ? "？" : info.Data.start_time.ToString(info.DrawHours ? "HH\\:mm" : "mm");
                double useHeight = sizeNormal / 3 + RenderText(textDrawList, min, ItemFontTitle, sizeMin, drawRect, 0, 0, colorTitle);
                
                //番組情報
                if (info.Data.ShortInfo != null)
                {
                    //タイトル、info.DrawHoursの際の追加インデントは仮。1行目だけインデント変えるのは面倒そうなので、フォント毎に「00:」相当のスペース文字数を調べるか
                    string title = (info.DrawHours ? "　 " : "") + CommonManager.ReplaceText(info.Data.ShortInfo.event_name.TrimEnd(), DictionaryTitle);
                    useHeight = sizeTitle / 3 + RenderText(textDrawList, title, ItemFontTitle, sizeTitle, drawRect, indentTitle, 0, colorTitle);

                    //説明
                    if (useHeight < drawRect.Height)
                    {
                        string detail = info.Data.ShortInfo.text_char.TrimEnd('\r', '\n');
                        detail += extraInfo == false || info.Data.ExtInfo == null ? "" : "\r\n\r\n" + info.Data.ExtInfo.text_char;
                        detail = CommonManager.ReplaceText(detail.TrimEnd(), DictionaryNormal);
                        if (detail != "") useHeight += sizeNormal / 3 + RenderText(textDrawList, detail, ItemFontNormal, sizeNormal, drawRect, indentNormal, useHeight, colorNormal);
                    }
                }

                SaveMaxRenderHeight(useHeight);
            }
        }
    }
}
