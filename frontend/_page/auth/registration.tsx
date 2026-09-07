"use client"
import AuthApi from "@/shared/api/AuthApi"
import { TDepartmentResponseDto } from "@/shared/api/department"
import { TGroupResponseDto } from "@/shared/api/group"
import { createApiErrorHandler } from "@/shared/ApiError/createApiErrorHandler"
import { Logo } from "@/shared/ui/Logo"
import { PasswordStregth } from "@/shared/ui/PasswordStregth"
import Wizard, { useWizard } from "@/shared/ui/wizard/Wizard"
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
	SvgIcon,
	OutlinedInput,
	InputAdornment,
	MenuItem,
	InputLabel,
	Tooltip,
} from "@mui/material"
import { useState } from "react"
import { useForm, useWatch, Controller } from "react-hook-form"
import { FormValues, LOGIN_FIELDS, PERSONAL_FIELDS } from "./fields"
import FormTextField from "./RegistrationTextField"
import FormRadioGroup from "./RegistrationRadioGroup"
import RegistrationButton from "./RegistrationButton"

interface Props {
	onToRegistration: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
	groups: TGroupResponseDto[]
	departments: TDepartmentResponseDto[]
}

const REQUIRED_PERSONAL_FIELDS = PERSONAL_FIELDS.filter(
	(value) => value != "patronymic",
)

type FulfieldValues = { [K in keyof Required<FormValues>]: boolean }

const Registration = (props: Props) => {
	const { focused, groups, onToRegistration, departments } = props

	const {
		register,
		handleSubmit,
		formState: { errors },
		control,
		trigger,
	} = useForm<FormValues>({
		defaultValues: {
			personRole: "STUDENT",
			group: "",
			department: "",
		},
	})

	const password = useWatch({
		control,
		name: "password",
		defaultValue: "",
	})

	const role = useWatch({
		control,
		name: "personRole",
		defaultValue: "STUDENT",
	})

	const isPersonalStepError = PERSONAL_FIELDS.some((field) => !!errors[field])

	const isLoginStepError = LOGIN_FIELDS.some((field) => !!errors[field])
	const handlerError = createApiErrorHandler()

	const onSubmit = handleSubmit(async (data) => {
		if (isLoginStepError || isPersonalStepError) {
			return
		}
		try {
			await AuthApi.register({
				login: data.login,
				password: data.password,
				email: data.email,
				firstName: data.firstName,
				lastName: data.lastName,
				patronymic: data.patronymic,
				rolesUuid: [role],
			})
		} catch (error) {
			handlerError(error)
		}
	})

	// const watchedValues = useWatch({ control }) as FormValues | undefined;
	// const isAllFilled = useMemo(() => {
	// 	if (!watchedValues) return false;
	// 	console.log("21")
	// 	// Проверяем, что все обязательные поля не пустые
	// 	const allKeys = [...LOGIN_FIELDS, ...REQUIRED_PERSONAL_FIELDS] as const;
	// 	return allKeys.every((key) => {

	// 		const value = watchedValues[key];

	// 		if (value === null || value === undefined) {
	// 			return false
	// 		};
	// 		if (typeof value === "string") {
	// 			if (value == "STUDENT") {
	// 				return watchedValues["group"]?.trim().length
	// 			} else if (value == "TEACHER") {
	// 				return watchedValues["department"]?.trim().length
	// 			}
	// 			return value.trim().length > 0
	// 		};
	// 	});
	// }, [watchedValues]);

	const [isPersonalStepCompleted, setIsPersonalStepCompleted] =
		useState(false)
	const [isLoginStepCompleted, setIsLoginStepCompleted] = useState(false)

	const [currentStep, setCurrentStep] = useState<number | string>(1)

	// текст тултипа кнопки "далее" на первом шаге
	const personalDataButtonTooltip =
		role === "STUDENT"
			? groups.length === 0
				? "Ошибка при связи с сервером. Групп нет"
				: null
			: departments.length === 0
				? "Ошибка при связи с сервером. Кафедр нет"
				: null

	const loginStepDisabled = personalDataButtonTooltip !== null

	return (
		// left-1 - чтобы не было видно границы между блоками при анимации
		<Box
			className="relative flex overflow-hidden "
			sx={(theme) => ({
				flex: 1,
				left: 1,

				[theme.breakpoints.down("md")]: {
					left: 0,
					flex: focused ? "auto" : "none",
					height: focused ? "100%" : "250px",
				},
			})}
		>
			<Stack
				className="absolute z-10 inset-0 flex-1 flex items-center  p-8 text-white self-stretch"
				sx={(theme) => ({
					transition: "clip-path 1s ease",
					clipPath: !focused
						? "circle(150% at center right)"
						: "circle(0% at center right)",
					backgroundColor: "primary.main",
					justifyContent: "center",

					[theme.breakpoints.down("md")]: {
						borderRadius: "0 0 64px 64px",
						justifyContent: "start",
						clipPath: !focused
							? "circle(150% at center top)"
							: "circle(0% at center top)",
					},
				})}
				spacing={4}
			>
				<Typography variant="h4">Вы в первый раз?</Typography>
				<Typography variant="h6">У вас ещё нет аккаунта?</Typography>
				<Button
					onClick={onToRegistration}
					variant="outlined"
					sx={{ backgroundColor: "white", fontSize: "16px" }}
				>
					зарегистрироваться
				</Button>
			</Stack>
			<Stack
				className="flex-1 py-5 px-7"
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
					<Typography variant="h4">Регистрация</Typography>
				</Box>

				<Wizard
					currentStep={currentStep}
					onStepChange={(step) => setCurrentStep(step)}
				>
					<Wizard.Step stepId={1}>
						<Wizard.StepHeader
							completed={isPersonalStepCompleted}
							errorMessage={
								isPersonalStepError
									? "Не все поля были корректно заполнены"
									: null
							}
						>
							Персональные данные
						</Wizard.StepHeader>
						<Wizard.StepContent>
							<form
								className="flex flex-col"
								onSubmit={(event) => event.preventDefault()}
							>
								<FormTextField
									label="Имя"
									size="small"
									fieldName="firstName"
								/>

								<FormTextField
									label="Фамилия"
									size="small"
									fieldName="lastName"
								/>

								<FormTextField
									label="Отчество"
									size="small"
									fieldName="patronymic"
								/>

								<FormTextField
									label="Email*"
									size="small"
									type="email"
									fieldName="email"
								/>

								<FormRadioGroup />
								<Tooltip title={personalDataButtonTooltip}>
									<span
										className={
											!!personalDataButtonTooltip
												? "cursor-not-allowed"
												: ""
										}
									>
										<Button
											className="w-full"
											variant="contained"
											color="primary"
											type="submit"
											onClick={async () => {
												const isValid =
													await trigger(
														PERSONAL_FIELDS,
													)
												if (isValid) {
													setIsPersonalStepCompleted(
														true,
													)
													setCurrentStep(2)
												}
											}}
											disabled={
												!!personalDataButtonTooltip
											}
										>
											Далее
										</Button>
									</span>
								</Tooltip>
							</form>
						</Wizard.StepContent>
					</Wizard.Step>

					<Wizard.Step disabled={loginStepDisabled}>
						<Wizard.StepHeader
							completed={isLoginStepCompleted}
							errorMessage={
								isLoginStepError
									? "Не все поля были корректно заполнены"
									: null
							}
						>
							Данные для входа
						</Wizard.StepHeader>
						<Wizard.StepContent>
							<form onSubmit={onSubmit} className="flex flex-col">
								<FormTextField
									label="Логин"
									size="small"
									fieldName="login"
								/>

								<FormTextField
									label="Пароль"
									size="small"
									type="password"
									fieldName="password"
									helperText={
										<PasswordStregth password={password} />
									}
								/>

								<FormTextField
									label="Повторите пароль"
									size="small"
									type="password"
									fieldName="passwordConfirm"
								/>

								<Tooltip title={"Регистрация не доступна"}>
									<span>
										<RegistrationButton />
									</span>
								</Tooltip>
							</form>
						</Wizard.StepContent>
					</Wizard.Step>
				</Wizard>
			</Stack>
		</Box>
	)
}

export default Registration
