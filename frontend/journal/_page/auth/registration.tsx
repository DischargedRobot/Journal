"use client"
import AuthApi from "@/shared/api/AuthApi"
import { createApiErrorHandler } from "@/shared/ApiError/createApiErrorHandler"
import { TGroup } from "@/shared/model/group"
import { TDepartment } from "@/shared/model/t-department"
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
import { useEffect, useState } from "react"
import { useForm, useWatch, Controller, UseFormTrigger } from "react-hook-form"

interface FormValues {
	login: string
	password: string
	passwordConfirm?: string
	firstName: string
	lastName: string
	patronymic?: string | null
	email?: string
	personRole: "STUDENT" | "TEACHER"
	department?: Department
	group?: string
}

interface Department {
	uuid: string
	name: string
}

interface Props {
	onToRegistration: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
	groups: TGroup[]
	departments: TDepartment[]
}

const PERSONAL_FIELDS: (keyof FormValues)[] = [
	"firstName",
	"lastName",
	"patronymic",
	"personRole",
	"group",
	"department",
]

const LOGIN_FIELDS: (keyof FormValues)[] = [
	"login",
	"password",
	"passwordConfirm",
]

const Registration = (props: Props) => {
	const { focused, groups, onToRegistration, departments } = props

	const {
		register,
		handleSubmit,
		formState: { errors },
		control,
		trigger,
	} = useForm<FormValues>({
		defaultValues: { personRole: "STUDENT", group: "" },
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
								<TextField
									variant="outlined"
									label="Имя*"
									size="small"
									{...register("firstName", {
										required: {
											value: true,
											message:
												"Поле обязательно для заполнения",
										},
									})}
									error={!!errors.firstName}
									helperText={
										errors.firstName?.message ?? " "
									}
								/>

								<TextField
									variant="outlined"
									label="Фамилия*"
									size="small"
									{...register("lastName", {
										required: {
											value: true,
											message:
												"Поле обязательно для заполнения",
										},
									})}
									error={!!errors.lastName}
									helperText={errors.lastName?.message ?? " "}
								/>

								<TextField
									variant="outlined"
									label="Отчество"
									size="small"
									{...register("patronymic")}
									helperText={" "}
								/>

								<TextField
									variant="outlined"
									label="Email"
									size="small"
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

								<FormControl
									component="fieldset"
									error={!!errors.personRole}
								>
									<FormLabel component="legend">
										Роль
									</FormLabel>
									<Controller
										name="personRole"
										control={control}
										rules={{ required: "Выберите роль" }}
										render={({ field }) => (
											<RadioGroup row {...field}>
												<FormControlLabel
													value="STUDENT"
													control={<Radio />}
													label="Студент"
												/>
												<FormControlLabel
													value="TEACHER"
													control={<Radio />}
													label="Преподаватель"
												/>
											</RadioGroup>
										)}
									/>
									<FormHelperText>
										{errors.personRole?.message ?? " "}
									</FormHelperText>
								</FormControl>

								{role === "STUDENT" && (
									<TextField
										select={groups.length > 0}
										variant="outlined"
										label={
											groups.length > 0
												? "Группа*"
												: "Групп нет"
										}
										size="small"
										{...register("group", {
											required:
												role === "STUDENT"
													? "Укажите группу"
													: false,
										})}
										disabled={groups.length === 0}
										error={!!errors.group}
										helperText={
											errors.group?.message ?? " "
										}
									>
										{groups.length > 0
											? groups.map((group) => (
													<MenuItem
														key={group.uuid}
														value={group.uuid}
													>
														{group.code}
													</MenuItem>
												))
											: null}
									</TextField>
								)}

								{role === "TEACHER" && (
									<TextField
										variant="outlined"
										label={
											departments.length > 0
												? "Кафедра*"
												: "Кафедр нет"
										}
										size="small"
										select={departments.length > 0}
										{...register("department", {
											required:
												role === "TEACHER"
													? "Укажите кафедру"
													: false,
										})}
										error={!!errors.department}
										disabled={departments.length === 0}
										helperText={
											errors.department?.message ?? " "
										}
									>
										{departments.length > 0
											? departments.map((department) => (
													<MenuItem
														key={department.uuid}
														value={department.uuid}
													>
														{department.name}
													</MenuItem>
												))
											: null}
									</TextField>
								)}

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
								<TextField
									variant="outlined"
									label="Логин*"
									size="small"
									{...register("login", {
										required: {
											value: true,
											message:
												"Поле обязательно для заполнения",
										},
									})}
									error={!!errors.login}
									helperText={errors.login?.message ?? " "}
								/>

								<FormControl error={!!errors.password}>
									<InputLabel htmlFor="password" size="small">
										Пароль*
									</InputLabel>
									<OutlinedInput
										label="Пароль*"
										id="password"
										size="small"
										type="password"
										{...register("password", {
											required: {
												value: true,
												message:
													"Поле обязательно для заполнения",
											},
										})}
										endAdornment={
											<InputAdornment position="end"></InputAdornment>
										}
									/>
									<FormHelperText className=" mt-1 mb-4">
										{errors.password ? (
											errors.password.message
										) : (
											<PasswordStregth
												password={password}
											/>
										)}
									</FormHelperText>
								</FormControl>

								<FormControl error={!!errors.passwordConfirm}>
									<InputLabel
										htmlFor="passwordConfirm"
										size="small"
									>
										Повторите пароль*
									</InputLabel>
									<OutlinedInput
										id="passwordConfirm"
										label="Повторите пароль*"
										size="small"
										type="password"
										{...register("passwordConfirm", {
											required: {
												value: true,
												message:
													"Поле обязательно для заполнения",
											},
											validate: (value) =>
												value === password ||
												"Пароли не совпадают",
										})}
										endAdornment={
											<InputAdornment position="end"></InputAdornment>
										}
									/>
									<FormHelperText className=" mt-1 mb-4">
										{errors.passwordConfirm
											? errors.passwordConfirm.message
											: " "}
									</FormHelperText>
								</FormControl>

								<Tooltip title={"Регистрация не доступна"}>
									<span>
										<Button
											className="w-full"
											variant="contained"
											color="primary"
											type="submit"
											disabled={true}
										>
											Зарегистрироваться
										</Button>
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
