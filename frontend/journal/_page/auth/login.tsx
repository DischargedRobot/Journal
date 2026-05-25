import { Stack, TextField, Typography, Button } from "@mui/material";
import { useForm } from "react-hook-form";

interface FormValues {
    login: string;
    password: string;
}

const Login = () => {
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<FormValues>();

    const onSubmit = handleSubmit((data) => {
        console.log('login', data);
    });

    return (
        <>
            <Stack
                className="flex-1 flex items-center justify-center p-8 text-white self-stretch"
                sx={{
                    backgroundColor: 'primary.main',
                }}
                spacing={4}
            >
                <Typography variant="h4">Регистрация</Typography>
                <Typography variant="h6">У вас ещё нет аккаунта?</Typography>
                <Button variant="outlined" sx={{ backgroundColor: 'white' }}>
                    Вы в первый раз?
                </Button>
            </Stack>

            <Stack
                className="flex-1 py-10 px-5"
                sx={{
                    bgcolor: 'secondary.main',
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
        </>
    );
};

export default Login;
