import AuthApi from "@/shared/api/AuthApi"
import { createApiErrorHandler } from "@/shared/ApiError/createApiErrorHandler"
import { Logo } from "@/shared/ui/Logo"
import {
	Stack,
	TextField,
	Typography,
	Button,
	Box,
	SvgIcon,
} from "@mui/material"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"

interface FormValues {
	login: string
	password: string
}

interface Props {
	onToLogin: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
}

const Login = (props: Props) => {
	const { focused, onToLogin } = props

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<FormValues>()

	const router = useRouter()

	const handlerError = createApiErrorHandler(router.push)

	const onSubmit = handleSubmit(async (data) => {
		try {
			const result = await AuthApi.login(data.login, data.password)
		} catch (error) {
			handlerError(error)
		}
	})

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
				</Box>
				<form onSubmit={onSubmit} className="flex flex-col  gap-4 ">
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
					<Button variant="contained" color="primary" type="submit">
						Войти
					</Button>
				</form>
			</Stack>
		</Box>
	)
}

export default Login
