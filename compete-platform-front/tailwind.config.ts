import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./app/**/*.tsx"],
  theme: {
    screens: {
      lg: { max: "1400px" },
      xl: { max: "1520px" },
      xxl: { max: "1720px" },
    },
    extend: {
      backgroundColor: {
        "rgba-black-40": "rgba(0, 0, 0, 0.4)",
      },
      fontFamily: {
        inter: "var(--font-inter)",
        montserrat: "var(--font-montserrat)",
      },
      boxShadow: {
        'user-card': '-4px 0px 0px 0px #151515',
      },
      colors: {
        saturateBlue: "#2563EB",
        deepBlue: "#090236",
        blue: "#59A8E0",
        secondaryBlue: "#80A4F2",
        "GRADIK-1": "#0F1116",
        "GRADIK-2": "#1A1B22",
        gray: "#545454",
        secondGray: "#9B9B9B",
        positive: "#00FF47",
        negative: "#FE0000",
        "gray-new": "#A6A6AA",
        "transparent-black": "#0A0A0A8A",
        darkBlue: "#28397F",
      },
      backgroundImage: {
        GRADIK: "linear-gradient(90deg, #0F1116 0%, #1A1B22 100%)",
        POSITIVCHIK:
          "linear-gradient(180deg, #09FA21 0%, rgba(76, 242, 18, 0.67) 100%)",
        GRAYCHIK: "linear-gradient(90deg, #232526 0%, #414345 100%)",
      },
      maxWidth: {
        'header-container': "1680px",
      }
    },
  },
  plugins: [],
};
export default config;
