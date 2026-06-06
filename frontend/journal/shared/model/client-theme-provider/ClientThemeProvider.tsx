"use client"
import React from "react"
import {
	createTheme,
	lighten,
	darken,
	ThemeProvider,
	PaletteColor,
	PaletteColorOptions,
} from "@mui/material/styles"

declare module "@mui/material/styles" {
	interface PaletteOptions {
		black: PaletteColorOptions
		default: PaletteColorOptions
		contrastingSecondary: PaletteColorOptions
	}

	interface Palette {
		contrastingSecondary: PaletteColor
		black: PaletteColor
		default: PaletteColor
	}
}

const base = createTheme()

const main = "#5B69E3"

const theme = createTheme({
	cssVariables: true,
	palette: {
		warning: {
			main: "#FF0000",
		},
		black: base.palette.augmentColor({
			color: { main: base.palette.grey[900] },
			name: "black",
		}),
		primary: {
			main,
			light: "#7AACF9",
			dark: "#3E4693",
			50: lighten(main, 0.9),
			100: lighten(main, 0.7),
			200: lighten(main, 0.5),
			300: lighten(main, 0.3),
			400: lighten(main, 0.15),
			500: main,
			600: darken(main, 0.08),
			700: darken(main, 0.16),
			800: darken(main, 0.24),
			900: darken(main, 0.4),
		}, // или расширить типы палитры
		secondary: {
			contrastText: "#282827",
			main: "#F3F3F3",
			light: "#FCFCFC",
			dark: "#EFEFEF",
		},
		contrastingSecondary: {
			main: "#8E8D8D",
			light: "#E3E3E3",
			dark: "#282827",
		},
		default: {
			main: "#8E8D8D",
		},
		mode: "light",
	},
})

export default function ClientThemeProvider(props: {
	children: React.ReactNode
}) {
	const { children } = props
	return <ThemeProvider theme={theme}>{children}</ThemeProvider>
}
