import type { Metadata } from "next";
import { Inter, Montserrat } from "next/font/google";

import "keen-slider/keen-slider.min.css";

import "./_styles/index.css";
import { RootProvider } from "./_fetures/lib/api/RootProvider";
import Script from "next/script";

const inter = Inter({
  subsets: [
    "cyrillic",
    "latin",
    "latin-ext",
    "cyrillic-ext",
    "greek",
    "greek-ext",
    "vietnamese",
  ],
  variable: "--font-inter",
  weight: ["100", "400", "500", "600", "700"],
});

const montserrat = Montserrat({
  subsets: ["cyrillic", "latin", "cyrillic-ext", "latin-ext", "vietnamese"],
  variable: "--font-montserrat",
  weight: ["300", "400", "500", "600", "800"],
});

export const metadata: Metadata = {
  title: "Compete",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ru">
      <body
        className={`${inter.variable} ${montserrat.variable}`}
      >
        <Script
          src="https://yookassa.ru/payouts-data/3.1.0/widget.js"
          strategy="beforeInteractive"
        ></Script>
        <RootProvider>{children}</RootProvider>
      </body>
    </html>
  );
}
