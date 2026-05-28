"use client"
import { Registration } from "@/_page/auth"
import { Login } from "@/_page/auth"
import { Container } from "@mui/material"
import { useState } from "react"

const AuthPage = () => {
	const [registrationOpen, setRegistrationOpen] = useState(false)
	return (
		<main className="content-center h-screen w-screen overflow-auto">
			<Container
				className="relative flex items-stretch justify-between p-0!  w-full overflow-clip"
				sx={(theme) => ({
					backgroundColor: "secondary.main",
					flexDirection: { xs: "column", md: "row" },
					borderRadius: "32px ",
					[theme.breakpoints.down("md")]: {
						minHeight: "100vh",
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
		</main>
	)
}

export default AuthPage
