"use client"
import React from "react"
import { createTheme, ThemeProvider } from "@mui/material/styles"

declare module "@mui/material/styles" {
	interface PaletteColor {
		black?: PaletteOptions["primary"]
		default?: PaletteOptions["primary"]
	}

	interface PaletteOptions {
		black?: PaletteOptions["primary"]
		default?: PaletteOptions["primary"]
	}
}

const base = createTheme()

const theme = createTheme({
	palette: {
		black: base.palette.augmentColor({
			color: { main: base.palette.grey[900] },
			name: "black",
		}),
		primary: { main: "#5b69e3" },
		secondary: {
			main: "#F3F3F3",
			light: "#FCFCFC",
			dark: "#EFEFEF",
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
