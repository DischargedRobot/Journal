import Button from "@mui/material/Button"
import Tooltip from "@mui/material/Tooltip"
import { memo } from "react"
import { Control, useWatch } from "react-hook-form"

export interface LoginFormValues {
	login: string
	password: string
}

interface Props {
	control: Control<LoginFormValues>
	responseError: boolean
	loading: boolean
}

const LoginButton = (props: Props) => {
	const { control, responseError, loading } = props
	const [login, password] = useWatch({
		control,
		name: ["login", "password"],
	})

	const isValid =
		responseError === false &&
		(login?.length ?? 0) > 0 &&
		(password?.length ?? 0) > 0

	const errorMessage =
		login?.length === 0 && password?.length === 0
			? "Введите логин и пароль"
			: login?.length === 0
				? "Введите логин"
				: password?.length === 0
					? "Введите пароль"
					: responseError
						? "Неверный логин или пароль"
						: ""
	console.log(errorMessage, "errorMessage")
	return (
		<Tooltip title={errorMessage}>
			<span>
				<Button
					className="w-full"
					variant="contained"
					color="primary"
					type="submit"
					disabled={!isValid}
					loading={loading}
				>
					Войти
				</Button>
			</span>
		</Tooltip>
	)
}

export default memo(LoginButton)
