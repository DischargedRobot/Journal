import { PasswordStregth } from "@/shared/ui/PasswordStregth"
import {
    Box,
    Button,
    Stack,
    TextField,
    Typography,
    FormControl,
    FormLabel,
    RadioGroup,
    FormControlLabel,
    Radio,
    FormHelperText,
} from "@mui/material"
import { useForm, useWatch, Controller } from "react-hook-form"
interface FormValues {
    login: string
    password: string
    firstName: string
    lastName: string
    patronymic?: string | null
    email?: string
    personRole: "STUDENT" | "TEACHER"
    department?: Department
    group?: Department
}

interface Department {
    uuid: string
    name: string
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
        formState: { errors },
        control,
    } = useForm<FormValues>()
    const password = useWatch({ control, name: 'password', defaultValue: '' })
    const role = useWatch({ control, name: 'personRole', defaultValue: 'STUDENT' })

    const onSubmit = handleSubmit((data) => {
        console.log(data)
    })
    return (
        // left-1 - чтобы не было видно границы между блоками при анимации
        <Box className="relative left-1 flex-1 flex overflow-hidden p-5">
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
                <form onSubmit={onSubmit} className="flex flex-col">
                    <TextField
                        variant="outlined"
                        label="Имя"
                        {...register("firstName", {
                            required: {
                                value: true,
                                message: "Поле обязательно для заполнения",
                            },
                        })}
                        error={!!errors.firstName}
                        helperText={errors.firstName?.message ?? " "}
                    />

                    <TextField
                        variant="outlined"
                        label="Фамилия"
                        {...register("lastName", {
                            required: {
                                value: true,
                                message: "Поле обязательно для заполнения",
                            },
                        })}
                        error={!!errors.lastName}
                        helperText={errors.lastName?.message ?? " "}
                    />

                    <TextField
                        variant="outlined"
                        label="Отчество"
                        {...register("patronymic")}
                        helperText={" "}
                    />

                    <TextField
                        variant="outlined"
                        label="Email"
                        type="email"
                        {...register("email", {
                            pattern: {
                                value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                message: "Неверный email",
                            },
                        })}
                        error={!!errors.email}
                        helperText={errors.email?.message ?? " "}
                    />

                    <FormControl component="fieldset" error={!!errors.personRole}>
                        <FormLabel component="legend">Роль</FormLabel>
                        <Controller
                            name="personRole"
                            control={control}
                            rules={{ required: 'Выберите роль' }}
                            render={({ field }) => (
                                <RadioGroup row {...field}>
                                    <FormControlLabel value="STUDENT" control={<Radio />} label="Студент" />
                                    <FormControlLabel value="TEACHER" control={<Radio />} label="Преподаватель" />
                                </RadioGroup>
                            )}
                        />
                        <FormHelperText>{errors.personRole?.message ?? " "}</FormHelperText>
                    </FormControl>

                    {role === 'STUDENT' && (
                        <TextField
                            variant="outlined"
                            label="Группа"
                            {...register('group', {
                                required: role === 'STUDENT' ? 'Укажите группу' : false,
                            })}
                            error={!!errors.group}
                            helperText={errors.group?.message ?? " "}
                        />
                    )}

                    {role === 'TEACHER' && (
                        <TextField
                            variant="outlined"
                            label="Кафедра"
                            {...register('department', {
                                required: role === 'TEACHER' ? 'Укажите кафедру' : false,
                            })}
                            error={!!errors.department}
                            helperText={errors.department?.message ?? " "}
                        />
                    )}

                    <TextField
                        variant="outlined"
                        label="Логин"
                        {...register("login", {
                            required: {
                                value: true,
                                message: "Поле обязательно для заполнения",
                            },
                        })}
                        error={!!errors.login}
                        helperText={errors.login?.message ?? " "}
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
                        error={!!errors.password}
                        helperText={errors.password ? errors.password.message : <PasswordStregth password={password} />}
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
