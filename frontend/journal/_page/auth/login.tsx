import { Stack, TextField, Typography, Button, Box } from "@mui/material"
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

    const onSubmit = handleSubmit((data) => {
        console.log("login", data)
    })

    return (
        // right-1 - чтобы не было видно границы между блоками при анимации
        <Box className="relative right-1 flex-1 flex overflow-hidden">
            <Stack
                className="absolute z-10 inset-0 flex-1 flex items-center justify-center p-8 text-white self-stretch"
                sx={{
                    transition: "clip-path 1s ease",
                    clipPath: !focused
                        ? "circle(150% at center left)"
                        : "circle(0% at center left)",
                    backgroundColor: "primary.main",
                    // display: focused ? 'flex' : 'none',
                }}
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
                className="flex-1 py-10 px-5"
                sx={{
                    transitionProperty: focused ? "none" : "visibility",
                    transitionDelay: "1s",
                    visibility: focused ? "visible" : "hidden",
                    bgcolor: "secondary.main",
                }}
                spacing={4}
            >
                <Typography variant="h4">Авторизация</Typography>
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
                    />
                    <Typography>Забыли пароль?</Typography>
                    <Button variant="contained" color="primary" type="submit">
                        Войти
                    </Button>
                </form>
            </Stack>
        </Box>
    )
}

export default Login
