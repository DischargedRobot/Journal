'use client'
import { Button, Container, Stack, TextField, Typography } from "@mui/material";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import { useForm } from "react-hook-form";

interface FormValues {
    login: string
    password: string
}

const base = createTheme()

declare module '@mui/material/styles' {
    interface PaletteColor {
        black?: PaletteOptions['primary'];
    }

    interface PaletteOptions {
        black?: PaletteOptions['primary'];
    }
}

const theme = createTheme({
    palette: {
        black: base.palette.augmentColor({
            color: { main: base.palette.grey[900] },
            name: "black",
        }),
        primary: {
            main: "#5b69e3",
        },
        mode: "light",
    },
})
const AuthPage = () => {
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<FormValues>();

    const onSubmit = handleSubmit((data) => {
        console.log(data);
    });

    return (
        <ThemeProvider theme={theme}>
            <Container className="flex items-stretch justify-between gap-6 p-0! w-full max-w-5xl bg-gray-100 rounded-3xl overflow-hidden ">

                <div
                    className="flex-1 flex items-center justify-center p-8 bg-primary text-white self-stretch"
                >
                    <Stack
                        className="items-center justify-center"
                        spacing={4}
                    >
                        <Typography variant="h4">Регистрация</Typography>
                        <Typography variant="h6">У вас ещё нет аккаунта?</Typography>
                        <Button variant="outlined" sx={{ backgroundColor: "white" }}>
                            Зарегистрироваться
                        </Button>
                    </Stack>
                </div>
                <Stack spacing={4} className="flex-1 py-10 px-5">
                    <Typography variant="h4">Авторизация</Typography>
                    <form onSubmit={onSubmit} className="flex flex-col gap-4 ">
                        <TextField
                            variant="outlined"
                            label="Логин"

                            {...register("login", {
                                required: {
                                    value: true,
                                    message: "Поле обязательно для заполнения"
                                }
                            })}
                        />
                        <TextField
                            variant="outlined"
                            label="Пароль"
                            type="password"
                            {...register("password", {
                                required: {
                                    value: true,
                                    message: "Поле обязательно для заполнения"
                                }
                            })}
                        />
                        <Typography>Забыли пароль?</Typography>
                        <Button variant="contained" color="primary" type="submit">
                            Войти
                        </Button>
                    </form>
                </Stack>
            </Container>
        </ThemeProvider>
    );
}

export default AuthPage;