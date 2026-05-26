"use client"
import { Registration } from "@/_page/auth"
import { Login } from "@/_page/auth"
import { Logo } from "@/shared/ui/Logo"
import { Container, SvgIcon } from "@mui/material"
import { createTheme, ThemeProvider } from "@mui/material/styles"
import { useState } from "react"

const base = createTheme()

declare module "@mui/material/styles" {
	interface PaletteColor {
		black?: PaletteOptions["primary"]
	}

	interface PaletteOptions {
		black?: PaletteOptions["primary"]
	}
}

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
		mode: "light",
	},
})
const AuthPage = () => {
	const [registrationOpen, setRegistrationOpen] = useState(false)
	return (
		<ThemeProvider theme={theme}>
			<Container
				className="relative flex items-stretch justify-between p-0!  w-full overflow-clip"
				sx={(theme) => ({
					backgroundColor: "secondary.main",
					flexDirection: { xs: "column", md: "row" },
					borderRadius: "32px ",
					[theme.breakpoints.down("md")]: {
						height: "100vh",
						borderRadius: "0",
					},
				})}
			>
				<Registration
					focused={registrationOpen}
					onToRegistration={() => setRegistrationOpen(true)}
				/>
				<Login
					focused={!registrationOpen}
					onToLogin={() => setRegistrationOpen(false)}
				/>
			</Container>
		</ThemeProvider>
	)
}

export default AuthPage
