using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public static class AnimationHelper
    {
        public static void NavigateWithFade(Frame frame, Page newPage)
        {
            // 현재 페이지를 페이드 아웃
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.Completed += (s, a) =>
            {
                // 페이드 아웃 완료 후 새 페이지로 전환
                frame.Content = newPage;

                // 새 페이지를 페이드 인
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                newPage.BeginAnimation(Page.OpacityProperty, fadeIn);
            };

            // 현재 페이지에 페이드 아웃 애니메이션 적용
            if (frame.Content is Page currentPage)
            {
                currentPage.BeginAnimation(Page.OpacityProperty, fadeOut);
            }
            else
            {
                // 현재 페이지가 없으면 바로 새 페이지로 전환
                frame.Content = newPage;

                // 새 페이지를 페이드 인
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                newPage.BeginAnimation(Page.OpacityProperty, fadeIn);
            }

        }

        public static void ShowLoginWithFade(MainWindow mainWindow, Page loginPage)
        {
            // 로그인 페이지를 페이드 인
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            loginPage.Opacity = 0;
            loginPage.BeginAnimation(Page.OpacityProperty, fadeIn);

            // 로그인 페이지를 화면에 표시
            mainWindow.frame.Content = loginPage;
        }
    }
}
