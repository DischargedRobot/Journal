import { Box, Button, Stack, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
interface FormValues {
    login: string
    password: string
}

const Registration = () => {

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<FormValues>();

    const onSubmit = handleSubmit((data) => {
        console.log(data);
    });
    return (
        <Box className="flex rounded-3xl overflow-clip"
            sx={{
                bgcolor: 'secondary.main',
            }}
        >
            <Stack spacing={4} className="flex-1 py-10 px-5">
                <Typography variant="h4">Регистрация</Typography>
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
                        Зарегистрироваться
                    </Button>
                </form>
            </Stack>
            <Stack
                className="flex-1 flex items-center justify-center p-8 bg-primary text-white self-stretch"
                spacing={4}
            >
                <Typography variant="h4">С возвращением!</Typography>
                <Typography variant="h6">У вас уже есть аккаунт?</Typography>
                <Button variant="outlined" className="" sx={{
                    backgroundColor: "white"
                }}>
                    Войти
                </Button>
            </Stack>
        </Box>
    );
}

export default Registration;