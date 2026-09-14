"use client"
import { AuthApi } from "@/shared/api/auth"
import { TDepartmentResponseDto } from "@/shared/api/department"
import { TGroupResponseDto } from "@/shared/api/group"
import { ApiErrors, createApiErrorHandler } from "@/shared/api/api-error"
import { Logo } from "@/shared/ui/Logo"
import { PasswordStregth } from "@/shared/ui/PasswordStregth"
import Wizard from "@/shared/ui/wizard/Wizard"
import { Box, Button, Stack, Typography, SvgIcon, Tooltip } from "@mui/material"
import { useState } from "react"
import { useForm, useWatch } from "react-hook-form"
import {
	FormValues,
	LOGIN_FIELDS,
	PERSONAL_FIELDS,
	REQUIRED_FIELDS,
} from "./fields"
import { useShallow } from "zustand/shallow"

import FormTextField from "./RegistrationTextField"
import FormRadioGroup from "./RegistrationRadioGroup"
import RegistrationButton from "./RegistrationButton"
import useRegistrationFormStore from "./model/useRegistrationFormStore"
import { TRole } from "@/shared/model/role"
import { useRouter } from "next/navigation"

interface Props {
	onToRegistration: (event: React.MouseEvent<HTMLButtonElement>) => void
	focused: boolean
	groups: TGroupResponseDto[]
	departments: TDepartmentResponseDto[]
	roles: TRole[]
}

const REQUIRED_PERSONAL_FIELDS = PERSONAL_FIELDS.filter(
	(value) => value != "patronymic",
)

type FulfieldValues = { [K in keyof Required<FormValues>]: boolean }

const Registration = (props: Props) => {
	const { focused, groups, onToRegistration, departments, roles } = props
	console.log(roles, "roles Registration")

	const {
		register,
		handleSubmit,
		formState: { errors },
		control,
		trigger,
	} = useForm<FormValues>({
		defaultValues: {
			personRole: "СТУДЕНТ",
			group: "",
			department: "",
		},
	})
	const password = useWatch({
		control,
		name: "password",
		defaultValue: "",
	})

	const isPersonalStepError = PERSONAL_FIELDS.some((field) => !!errors[field])
	const updateField = useRegistrationFormStore((state) => state.updateField)
	const isLoginStepError = LOGIN_FIELDS.some((field) => !!errors[field])
	const handlerRegistrationError = createApiErrorHandler([
		{
			error: ApiErrors.CONFLICT,
			handler: (error) => {
				if (error.field?.toLowerCase() === "login") {
					updateField("login", "Логин уже занят")
				} else if (error.field?.toLowerCase() === "email") {
					updateField("email", "Email уже занят")
				}
			},
		},
	])

	const formValues = useRegistrationFormStore(
		useShallow((state) => state.formValues),
	)

	const router = useRouter()
	const onSubmit = handleSubmit(async () => {
		if (isLoginStepError || isPersonalStepError) {
			return
		}
		try {
			// находим роль СТУДЕНТ или ПРЕПОДАВАТЕЛЬ
			const selectedRole =
				roles.find(
					(r) =>
						r.name.toUpperCase().trim() ===
						formValues.personRole.value.toUpperCase().trim(),
				)?.uuid ?? null
			console.log(
				selectedRole,
				formValues.personRole.value,
				roles,
				"sadas",
			)
			await AuthApi.register({
				login: formValues.login.value,
				password: formValues.password.value,
				email: formValues.email.value,
				firstName: formValues.firstName.value,
				lastName: formValues.lastName.value,
				patronymic: formValues.patronymic.value,
				rolesUuid: selectedRole == null ? [] : [selectedRole],
			})

			router.push("/journal")
		} catch (error) {
			handlerRegistrationError(error)
		}
	})

	// console.log("reg")
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
	// 			if (value == "СТУДЕНТ") {
	// 				return watchedValues["group"]?.trim().length
	// 			} else if (value == "ПРЕПОДАВАТЕЛЬ") {
	// 				return watchedValues["department"]?.trim().length
	// 			}
	// 			return value.trim().length > 0
	// 		};
	// 	});
	// }, [watchedValues]);

	const isLoginStepCompleted = useRegistrationFormStore((state) => {
		return LOGIN_FIELDS.filter((item) =>
			REQUIRED_FIELDS.includes(item),
		).every((fieldName) => {
			const field = state.formValues[fieldName]
			return (
				((field.value.trim() !== "" && field.required) ||
					!field.required) &&
				field.error === ""
			)
		})
	})

	const isPersonalStepCompleted = useRegistrationFormStore((state) => {
		return PERSONAL_FIELDS.filter((item) =>
			REQUIRED_FIELDS.includes(item),
		).every((fieldName) => {
			const field = state.formValues[fieldName]
			if (fieldName == "personRole") {
				const personRole = state.formValues.personRole.value
				if (personRole === "СТУДЕНТ") {
					const group = state.formValues.group
					console.log(group.value, personRole, "СТУДЕНТыыы")
					return (
						groups.map((g) => g.code).includes(group.value ?? "") &&
						group.error.length == 0
					)
				} else {
					const department = state.formValues.department
					console.log(department.value)

					return (
						departments
							.map((d) => d.code)
							.includes(department.value ?? "") &&
						department.error.length == 0
					)
				}
			}

			return (
				((field.value?.trim() !== "" && field.required) ||
					!field.required) &&
				field.error.length == 0
			)
		})
	})
	console.log(
		groups,
		isPersonalStepCompleted,
		"isPersonalStepCompleted",
		formValues.group,
	)

	const [currentStep, setCurrentStep] = useState<number | string>(1)

	// текст тултипа кнопки "далее" на первом шаге
	const personalDataButtonTooltip =
		formValues.personRole.value === "СТУДЕНТ"
			? groups.length === 0
				? "Ошибка при связи с сервером. Групп нет"
				: null
			: departments.length === 0
				? "Ошибка при связи с сервером. Кафедр нет"
				: null

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

								<FormRadioGroup
									groups={groups}
									departments={departments}
								/>
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
											onClick={() => setCurrentStep(2)}
											disabled={!isPersonalStepCompleted}
										>
											Далее
										</Button>
									</span>
								</Tooltip>
							</form>
						</Wizard.StepContent>
					</Wizard.Step>

					<Wizard.Step disabled={!isPersonalStepCompleted}>
						<Wizard.StepHeader
							completed={isLoginStepCompleted}
							errorMessage={
								isLoginStepError
									? "Не все поля были корректно заполнены"
									: null
							}
						>
							Учётные данные
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
