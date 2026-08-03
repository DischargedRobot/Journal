"use client"

import { Login, Registration } from "@/_page/auth"
import { TDepartmentResponseDto } from "@/shared/api/department/DepartmentApi"
import { TGroupResponseDto } from "@/shared/api/group/GroupApi"
import { Container } from "@mui/material"
import { useState } from "react"


interface Props {
	groups: TGroupResponseDto[]
	departments: TDepartmentResponseDto[]
}

export const AuthClient = (props: Props) => {
	const { groups, departments } = props
	const [registrationOpen, setRegistrationOpen] = useState(false)

	return (
		<Container
			className="relative flex items-stretch justify-between p-0! w-full overflow-clip"
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
				groups={groups}
				departments={departments}
				onToRegistration={() => setRegistrationOpen(true)}
			/>
			<Login
				focused={!registrationOpen}
				onToLogin={() => setRegistrationOpen(false)}
			/>
		</Container>
	)
}
