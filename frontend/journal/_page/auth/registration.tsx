import { PasswordStregth } from "@/shared/ui/PasswordStregth"
import { Box, Button, Stack, TextField, Typography } from "@mui/material"
import { useForm } from "react-hook-form"
interface FormValues {
	login: string
	password: string
}

interface Props {
	onToRegistration: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
}

const Registration = (props: Props) => {
	const { focused, onToRegistration } = props

	const {
		register,
		handleSubmit,
		watch,
		formState: { errors },
	} = useForm<FormValues>()

	const onSubmit = handleSubmit((data) => {
		console.log(data)
	})
	return (
		// left-1 - чтобы не было видно границы между блоками при анимации
		<Box className="relative left-1 flex-1 flex overflow-hidden">
			<Stack
				className="absolute z-10 inset-0 flex-1 flex items-center justify-center p-8 text-white self-stretch"
				sx={{
					transition: "clip-path 1s ease",
					clipPath: !focused
						? "circle(150% at center right)"
						: "circle(0% at center right)",
					backgroundColor: "primary.main",
					// display: focused ? 'flex' : 'none',
				}}
				spacing={4}
			>
				<Typography variant="h4">Вы в первый раз?</Typography>
				<Typography variant="h6">У вас ещё нет аккаунта?</Typography>
				<Button
					onClick={onToRegistration}
					variant="outlined"
					sx={{ backgroundColor: "white" }}
				>
					зарегистрироваться
				</Button>
			</Stack>
			<Stack
				className="flex-1 py-10 px-5"
				sx={{
					transitionProperty: focused ? "none" : "visibility",
					transitionDelay: "1s",
					visibility: focused ? "visible" : "hidden",
					bgcolor: "secondary.main",
				}}
				spacing={4}
			>
				<Typography variant="h4">Регистрация</Typography>
				<form onSubmit={onSubmit} className="flex flex-col gap-4 ">
					<TextField
						variant="outlined"
						label="Логин"
						{...register("login", {
							required: {
								value: true,
								message: "Поле обязательно для заполнения",
							},
						})}
					/>
					<TextField
						variant="outlined"
						label="Пароль"
						type="password"
						{...register("password", {
							required: {
								value: true,
								message: "Поле обязательно для заполнения",
							},
						})}
						helperText={
							<PasswordStregth password={watch("password")} />
						}
					/>
					<Typography>Забыли пароль?</Typography>
					<Button variant="contained" color="primary" type="submit">
						Зарегистрироваться
					</Button>
				</form>
			</Stack>
		</Box>
	)
}

export default Registration
