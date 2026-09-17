"use client"

import { AuthApi } from "@/shared/api/auth"
import { ApiErrors, createApiErrorHandler } from "@/shared/api/api-error"
import { Logo } from "@/shared/ui/Logo"
import {
	Stack,
	TextField,
	Typography,
	Button,
	Box,
	SvgIcon,
	InputAdornment,
	IconButton,
} from "@mui/material"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { useState } from "react"
import LoginButton, { LoginFormValues } from "./LoginButton"
import { Visibility, VisibilityOff } from "@mui/icons-material"

interface Props {
	onToLogin: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
}

const Login = (props: Props) => {
	const { focused, onToLogin } = props

	const { register, handleSubmit, control } = useForm<LoginFormValues>({
		defaultValues: {
			login: "",
			password: "",
		},
	})
	const router = useRouter()
	const [errorMessageResponse, setErrorMessageResponse] = useState("\u00A0")
	const handlerError = createApiErrorHandler(
		[
			{
				error: ApiErrors.UNAUTHORIZED,
				handler: () => {
					setErrorMessageResponse("Неверный логин или пароль")
				},
			},
		],
		router.push,
	)

	const onSubmit = handleSubmit(async (data) => {
		setLoading(true)
		try {
			await AuthApi.logIn(data.login, data.password)
			router.push("/journal")
		} catch (error) {
			handlerError(error)
		} finally {
			setLoading(false)
		}
	})

	const [loading, setLoading] = useState(false)

	const [showPassword, setShowPassword] = useState(false)
	return (
		// right-1 - чтобы не было видно границы между блоками при анимации
		<Box
			className="relative right-1 flex overflow-hidden py-5 px-7"
			sx={(theme) => ({
				flex: 1,
				right: 1,
				borderRadius: "0 32px 32px 0",
				[theme.breakpoints.down("md")]: {
					right: 0,
					flex: focused ? "auto" : "none",
					height: focused ? "730px" : "250px",
					borderRadius: "0",
				},
			})}
		>
			<Stack
				className="absolute z-10 inset-0 flex items-center  p-8  text-white self-stretch"
				sx={(theme) => ({
					transition: "clip-path 1s ease",
					clipPath: !focused
						? "circle(150% at center left)"
						: "circle(0% at center left)",
					backgroundColor: "primary.main",
					justifyContent: "center",

					[theme.breakpoints.down("md")]: {
						borderRadius: "64px 64px 0 0",
						justifyContent: "end",
						clipPath: !focused
							? "circle(150% at center bottom)"
							: "circle(0% at center bottom)",
					},
					// display: focused ? 'flex' : 'none',
				})}
				spacing={4}
			>
				<Typography variant="h4">С возвращением!</Typography>
				<Typography variant="h6">У вас уже есть аккаунт?</Typography>
				<Button
					variant="outlined"
					onClick={onToLogin}
					sx={{ backgroundColor: "white" }}
				>
					Войти
				</Button>
			</Stack>

			<Stack
				className="flex-1 flex justify-center py-10 px-5"
				sx={(theme) => ({
					visibility: focused ? "visible" : "hidden",
					bgcolor: "secondary.main",

					[theme.breakpoints.up("md")]: {
						transitionProperty: focused ? "none" : "visibility",
						transitionDelay: "1s",
					},
				})}
				spacing={4}
			>
				<Box className="flex flex-col items-center gap-2">
					<SvgIcon
						className="rounded-[50%] p-2"
						sx={{
							fontSize: 100,
						}}
						viewBox="0 0 63 69"
					>
						<Logo />
					</SvgIcon>
					<Typography variant="h4">Авторизация</Typography>
					<Typography variant="body1" color="error">
						{errorMessageResponse || "\u00A0"}
					</Typography>
				</Box>
				<form onSubmit={onSubmit} className="flex flex-col  gap-4 ">
					<TextField
						{...register("login", {
							required: true,
							onChange: () => {
								if (errorMessageResponse !== "\u00A0") {
									setErrorMessageResponse("\u00A0")
								}
							},
						})}
						variant="outlined"
						label="Логин"
						required
						error={errorMessageResponse !== "\u00A0"}
					/>
					<TextField
						{...register("password", {
							required: true,
							onChange: () => {
								if (errorMessageResponse !== "\u00A0") {
									setErrorMessageResponse("\u00A0")
								}
							},
						})}
						variant="outlined"
						label="Пароль"
						required
						type={showPassword ? "text" : "password"}
						error={errorMessageResponse !== "\u00A0"}
						slotProps={{
							input: {
								endAdornment: (
									<InputAdornment position="end">
										<IconButton
											onMouseDown={(event) => {
												event.preventDefault()
											}}
											onClick={() =>
												setShowPassword(
													(state) => !state,
												)
											}
										>
											{showPassword ? (
												<VisibilityOff />
											) : (
												<Visibility />
											)}
										</IconButton>
									</InputAdornment>
								),
							},
						}}
					/>
					<Typography
						variant="subtitle1"
						sx={{
							"&:hover": { textDecoration: "underline" },
							cursor: "pointer",
						}}
					>
						Забыли пароль?
					</Typography>
					<LoginButton
						loading={loading}
						control={control}
						responseError={errorMessageResponse !== "\u00A0"}
					/>
				</form>
			</Stack>
		</Box>
	)
}

export default Login
